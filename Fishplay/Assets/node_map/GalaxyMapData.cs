using System;
using NUnit.Framework;
using UnityEngine;
using Random = UnityEngine.Random;

public class GalaxyMapData : MonoBehaviour
{
	private GalaxyData _data;

	private const int _SIZE = 3;		// 先用3x3的大小，以后再考虑要不要扩容

	private JumpNodeData _currentPos;

	public GalaxyData Initialize()
	{
		Randomize();
		return _data;
	}

	void OnDrawGizmos()
	{
		if (_data == null)
			return;

		// draw frames
		Gizmos.DrawWireCube(Vector3.zero, new Vector3(3,1,3));
		Gizmos.color = Color.green;
		for (int i = 0; i < 9; i++)
		{
			int y = Mathf.FloorToInt(i/3) - 1;
			int x = i%3 - 1;
			Gizmos.DrawWireCube(new Vector3(x, 0, y), Vector3.one);
			if (i%3 == 0)
			{
				y++;
				x = -1;
			}
			else
			{
				x++;
			}
		}

		// draw node
		int size = (int)Mathf.Sqrt(_data.Frames.Length);
		for (int i = 0; i < size; i++)
		{
			for (int j = 0; j < size; j++)
			{
				GalaxyFrameBox box = _data.Frames[i, j];
				const float OFFSET = 0.5f;
				int x = box.Coord[0] - 1;
				int y = box.Coord[1] - 1;
				foreach (JumpNodeData d in box.NodeData)
				{
					Gizmos.color = Color.yellow;
					if (d.NodeType == JumpNodeType.JumpGate)
						Gizmos.color = Color.magenta;
					else if (d.NodeType == JumpNodeType.StartPoint)
						Gizmos.color = Color.green;
					Gizmos.DrawWireSphere(new Vector3(x + d.PositionInBlock.x - OFFSET, d.PositionInBlock.y - OFFSET, y + d.PositionInBlock.z - OFFSET), 0.02f);
				}
			}
		}

		// draw solar
		Gizmos.color = Color.blue;
		Gizmos.DrawWireSphere(Vector3.zero, 0.3f);
	}

	[ContextMenu("Randomize data")]
	void Randomize()
	{
		_data = new GalaxyData(3);
	}

	[ContextMenu("Show Neighbor")]
	void Debug_ShowNeighbor()
	{
		int x = Random.Range(0, _SIZE);
		int y = Random.Range(0, _SIZE);
		Debug.Log(string.Format("checking frame {0}:{1}...", x,y));

		const float DURATION = 3.0f;

		DrawFrameOnCoord(x,y, 0.3f, Color.green, DURATION);

		GalaxyFrameBox[] box_arr = _data.GetConnectingBlock(x,y);
		foreach(GalaxyFrameBox b in box_arr)
		{
			// Debug.Log(string.Format("neighbor coord: {0}:{1}", b.Coord[0],b.Coord[1]));
			DrawFrameOnCoord(b.Coord[0], b.Coord[1], 0.3f, Color.cyan, DURATION);
		// Gizmos.DrawWireSphere(new Vector3(x - 0.5f, 0, y - 0.5f), 0.3f);
		}
	}

	void DrawFrameOnCoord(int x, int y, float length, Color clr, float duration)
	{
		float len = length * 0.5f;
		Debug.DrawLine(new Vector3(x - len - 1,0,-(y - len) + 1), new Vector3(x + len - 1, 0, -(y - len) + 1), clr, duration);
		Debug.DrawLine(new Vector3(x + len - 1, 0, -(y - len) + 1), new Vector3(x + len - 1, 0, -(y + len) + 1), clr, duration);
		Debug.DrawLine(new Vector3(x + len - 1,0,-(y + len) + 1), new Vector3(x - len - 1, 0, -(y + len) + 1), clr, duration);
		Debug.DrawLine(new Vector3(x - len - 1,0,-(y + len) + 1), new Vector3(x - len - 1, 0, -(y - len) + 1), clr, duration);
	}
}

public class GalaxyData
{
	private GalaxyFrameBox[,] _frames;
	public GalaxyFrameBox[,] Frames { get { return _frames; } }

	/// <summary>
	/// 表示横向/纵向每个frame之间是否连通
	/// 0代表连通，1代表有阻隔
	/// 选择一个轴向，然后再选择随机出现两个隔断不会出现封闭空间
	/// TODO:目前先不考虑阻隔问题
	/// </summary>
	private int[,] _block_h = new int[3, 2];
	private int[,] _block_v = new int[2, 3];

	private bool _isFirstMap = false;
	private int _size;
	
	public GalaxyData(int size, bool isFirstMap = false)
	{
		_isFirstMap = isFirstMap;

		_size = size;

		_frames = new GalaxyFrameBox[size, size];
		int[] random_idx_array = GetGalaxyFlags(size);
		for(int i=0; i< size;i++)
		{
			for (int j = 0; j < size; j++)
			{
				// TODO: 每个frame先固定放2个node；之后改数据驱动的时候暴露出来
				FillBlock(i, j, 2
					, isFirstMap && i == random_idx_array[0] && j == random_idx_array[1]
					, ((i == random_idx_array[0] && j == random_idx_array[1])
					   || (i == random_idx_array[1] && j == random_idx_array[0])) && !isFirstMap);
			}
		}
	}

	// 获取开始位置和星门位置
	int[] GetGalaxyFlags(int size)
	{
		int[] candidates = {0, size - 1};
		int x = candidates[Random.Range(0, 2)];
		int y = candidates[Random.Range(0, 2)];
		int[] result = {x, y};
		return result;
	}

	int GetIndexOnEdge(int val)
	{
		if(val == 0)
			return val + 1;
		
		if(val == _size - 1)
			return val - 1;

		return -1;
	}

	/// <summary>
	/// 获取某个坐标临近（可连通的）block 引用
	/// TODO: 考虑单独起一个array专门用来更新和返回此方法引起的更新和返回。引用直接从_frames里面摘
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <returns></returns>
	public GalaxyFrameBox[] GetConnectingBlock(int x, int y)
	{
		GalaxyFrameBox[] result = null;

		int idx_size = _size - 1;
		
		Debug.Log(string.Format("Getting connecting blocks (size:{0})", _size));
		bool frame_in_corner = (x == 0 && y == 0)
			|| (x == idx_size && y == idx_size)
			|| (x == 0 && y == idx_size)
			|| (x == idx_size && y == 0);

		bool frame_on_edge = !frame_in_corner
		 && (x == 0 || x == idx_size || y == 0 || y == idx_size);

		// 所要获取的frame在四角，获取两个
		int tx = GetIndexOnEdge(x);
		int ty = GetIndexOnEdge(y);
		if(frame_in_corner)
		{
			return new [] {_frames[tx, y]
				, _frames[x, ty]};
		}
		// 所要获取的frame在边缘，获取三个
		else if (frame_on_edge)
		{
			// 贴顶端或者底端
			if(tx < 0)
				return new []{
					_frames[x-1,y]
					, _frames[x,ty]
					, _frames[x+1, y]
				};
			// 贴左右两端
			else if(ty < 0)
				return new []{
					_frames[x, y-1]
					, _frames[tx, y]
					, _frames[x, y+1]
				};
			else
			{
				throw new Exception("x, y index calc error occured");
			}
		}
		// 其他情况，获取四个
		else
		{
			return new []{
				_frames[x-1, y]
				,_frames[x+1, y]
				,_frames[x,y-1]
				,_frames[x,y+1]
			};
		}
	}

	void FillBlock(int m, int n, int count, bool isStart, bool isGate)
	{
		_frames[m, n] = new GalaxyFrameBox(Random.Range(1,count + 1), m, n, isStart, isGate);
	}
}

public class JumpNodeData
{
	public JumpNodeType NodeType = JumpNodeType.NotSet;
	public JumpNodePreset Preset;		// 该node的具体配置，从asset文件中获取。由解析配置文件的模块解析后生成关卡

	public  Vector3 PositionInBlock;		// 该node在所在frame内的坐标点位置。注意z值生成时给定即可，并不影响在2d galaxy map中的显示

	public string NodeName;
	public int[] CoordInGalaxy;

	public JumpNodeData(int x_coord, int y_coord)
	{
		CoordInGalaxy = new[]{x_coord, y_coord};
		NodeType = JumpNodeType.Skirmish;		// TODO: 先全都设置成遭遇战
		PositionInBlock = new Vector3(ClampedValue(), ClampedValue(), ClampedValue());
		NodeName = string.Format("NODE_{0}_{1}_{2},{3}", NodeType.ToString(), Random.Range(10000, 65534), CoordInGalaxy[0],CoordInGalaxy[1]);
	}

	float ClampedValue()
	{
		return Mathf.Clamp(Random.value, 0.2f, 0.8f);
	}
}

public enum JumpNodeType
{
	NotSet = 0,
	StartPoint = 5,
	Nothing = 10,
	LostFound = 20,
	Skirmish = 50,

	JumpGate = 100,
}

public class GalaxyFrameBox
{
	public int[] Coord;
	public JumpNodeData[] NodeData;

	private int _startNodeIdx = -1;
	private int _jumpGateOutIdx = -1;

	public GalaxyFrameBox(int nodeCount, int x, int y, bool isStartPool = false, bool isJumpGateOut = false)
	{
		ReFill(nodeCount, x, y);
		int idx = Random.Range(0, NodeData.Length);

		// 给定类型，是普通节点还是起始点还是星门
		// TODO: 普通节点以后使用正常的随机逻辑
		// 起始点使用Nothing类别
		if (isStartPool)
		{
			_startNodeIdx = idx;
			NodeData[idx].NodeType = JumpNodeType.StartPoint;
		}
		else if (isJumpGateOut)
		{
			_jumpGateOutIdx = idx;
			NodeData[idx].NodeType = JumpNodeType.JumpGate;
		}
		else
		{
			// TODO: 这里以后需要插入可参数化调节的跳点类型测定机制
			// TODO: 现在先全都用skirmish
			foreach (JumpNodeData d in NodeData)
			{
				if (Array.IndexOf(NodeData, d) == idx)
					continue;
				d.NodeType = JumpNodeType.Skirmish;
			}
		}
	}

	/// <summary>
	/// 随机返回一个jumpNode，作为返回数组index为0的值，如果当前frame不止一个那么将放到index>0的位置上
	/// </summary>
	/// <returns></returns>
	public JumpNodeData[] RandomPopOneAndItsFriends()
	{
		int rnd = Random.Range(0, NodeData.Length);
		JumpNodeData[] result = new JumpNodeData[NodeData.Length];
		result[0] = NodeData[rnd];
		int idx = 1;
		if(NodeData.Length == 0)
			throw new Exception("jump Node data empty, sth wrong happend");
		if (NodeData.Length == 1)
			return result;
		for (int i = 0; i < NodeData.Length; i++)
		{
			if (i == rnd)
				continue;
			result[idx] = NodeData[i];
			idx++;
		}
		return result;
	}

	public void ReFill(int nodeCount, int x, int y)
	{
		NodeData = new JumpNodeData[nodeCount];
		for(int i=0;i<nodeCount;i++)
			NodeData[i] = new JumpNodeData(x,y);
		Coord = new[] {x, y};
	}
}
