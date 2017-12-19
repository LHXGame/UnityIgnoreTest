using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MazeGenerate : MonoBehaviour {
    public GameObject Cubeprefabs;
    public int maxX = 21;//都为奇数
    public int maxY = 21;
    public int maxZ = 3;//高度
    private int[,,] graphic; //1为障碍 0为通路 -1代表障碍变为通路,-2代表0值的通路被访问过了
    private int isSelectedNodeNums = 0; //记录当前遍历了多少个0值的联通点
    private int dimension = 3;//当前的维数
    private int[,] dir = new int[6, 3] { { -1, 0,0 }, { 1, 0 ,0}, { 0, -1 ,0}, { 0, 1 ,0}, { 0,0,1}, { 0,0,-1} };//前后左右上下

    void Awake () {
        int totoalNums = initGraphic();
        printMaze();
        int startX = 1; int startY = 1; int startZ = 0;
        randomStartXY(ref startX, ref startY,ref startZ);
        Debug.Log(startX + " " + startY + " " + startZ);
        dfsGenMaze(startX,startY,startZ,totoalNums);
        createMaze();
        printMaze();
    }

    /// <summary>
    /// 随机一个值为0的开始通路
    /// </summary>
    private void randomStartXY(ref int startX, ref int startY,ref int startZ)
    {
        //只在第一层随机就好了 startZ设置为0即可
        startZ = 0;

        //随机一个值为0的开始通路
        //x:1 3 5 7 ... [maxX-1是否为奇数?maxX-1,maxX - 2]; 
        //y:1 3 5 7 ... [maxY-1是否为奇数?maxY-1,maxY - 2];
        int randomMaxX;
        if ((maxX - 1) % 2 == 0)
            randomMaxX = maxX - 2;
        else
            randomMaxX = maxX - 1;

        //x：1 3 5 7 ...randomMaxX 有多少个数
        int totalXNum = (randomMaxX - 1) / 2 + 1;
        //找第N个数
        int indexX = Random.Range(1, totalXNum + 1);
        startX = 1 + (indexX - 1) * 2;

        int randomMaxY;
        if ((maxY - 1) % 2 == 0)
            randomMaxY = maxY - 2;
        else
            randomMaxY = maxY - 1;

        //x：1 3 5 7 ...randomMaxY 有多少个数
        int totalYNum = (randomMaxY - 1) / 2 + 1;
        //找第N个数
        int indexY = Random.Range(1, totalYNum + 1);
        startY = 1 + (indexY - 1) * 2;
    }

    /// <summary>
    /// 初始化地图
    /// </summary>
    private int initGraphic()
    {
        isSelectedNodeNums = 0;
        dimension = 3;
        int totalNums = 0; //记录0值的节点多少个

        graphic = new int[maxX, maxY,maxZ];

        //第一层的初始化
        for(int i = 0; i < maxX; i+=2)
        {
            for(int j = 0; j < maxY; j++)
            {
                graphic[i, j, 0] = 1;        
            }
        }
        for (int i = 1; i < maxX - 1; i += 2)
        {
            int tmp = 1;
            for (int j = 0; j < maxY; j++)
            {
                graphic[i, j, 0] = tmp;
                if (tmp == 0)
                {
                    totalNums++;
                    tmp = 1;
                }
                else
                    tmp = 0;
            }
        }

        //Z层上的初始化
        for (int k = 1; k < maxZ; k++)
        {
            //外围四周
            for(int i = 0; i < maxX; i++)
            {
                graphic[i, 0, k] = 1;
                graphic[i, maxY - 1, k] = 1;
            }
            for(int j = 0; j < maxY; j++)
            {
                graphic[0, j, k] = 1;
                graphic[maxX - 1, j, k] = 1;
            }

            for(int i = 1; i < maxX - 1; i++)
            {
                for(int j = 1; j < maxY - 1; j++)
                {
                    if (graphic[i, j, k - 1] == 0)
                        graphic[i, j, k] = 1;
                    else
                    {
                        graphic[i, j, k] = 0;
                        totalNums++;
                    }                     
                }
            }
        }     
        return totalNums;       
    }

    /// <summary>
    /// 检查边界(外围不算在内)，不越界返回true,否则为false
    /// </summary>
    /// <returns></returns>
    bool checkBound(int x,int y,int z)
    {
        return x > 0 && x < maxX - 1 && y > 0 && y < maxY - 1 && z >=0 && z < maxZ;
    }

    /// <summary>
    /// 深度优先遍历生成迷宫,假定开始参数是一个通路(值0)
    /// </summary>
    /// <param name="startX">开始位置X</param>
    /// <param name="startY">开始位置Y</param>
    /// <param name="maxNodeNum">图初始化时的最大联通点数</param>
    void dfsGenMaze(int startX,int startY,int startZ,int maxNodeNum)
    {      
        //越界
        if (checkBound(startX, startY,startZ) == false) return;

        //0值通路被访问过
        graphic[startX, startY,startZ] = -2;
        isSelectedNodeNums++;

        //随机出0 - 3
        List<int> dirList = new List<int>();
        for (int i = 0; i < dimension * 2; i++)
            dirList.Add(i);
        List<int> randomdirList = new List<int>();
        //随机
        for(int i = 0; i < dimension * 2; i++)
        {
            int index = Random.Range(0, dirList.Count);
            randomdirList.Add(dirList[index]);
            dirList.Remove(dirList[index]);
        } 
        
        //四次方向搜索,但是得随机
        for(int i = 0; i < dimension * 2; i++)
        {
            //领近节点,领近节点肯定是墙
            int nextX = startX + dir[randomdirList[i],0];
            int nextY = startY + dir[randomdirList[i],1];
            int nextZ = startZ + dir[randomdirList[i],2];

            if (checkBound(nextX, nextY,nextZ)) //判断领近节点是否越界
            {
                if (graphic[nextX, nextY,nextZ] == -1) //领近节点是已经拆过的墙
                   continue;

                if (graphic[nextX, nextY,nextZ] == 1) //如果领近节点是墙,看墙对面是否是通路(并且通路要求是未访问过的，否则打通这面墙会形成回路)
                {                   
                    int oppositeX = nextX + dir[randomdirList[i], 0];
                    int oppositeY = nextY + dir[randomdirList[i], 1];
                    int oppositeZ = nextZ + dir[randomdirList[i], 2];
                    if (checkBound(oppositeX, oppositeY,oppositeZ)) //判断墙对面是否越界
                    {
                        //墙对面是通路
                        if (graphic[oppositeX, oppositeY,oppositeZ] == 0)
                        {
                            //把墙拆了，标记为通路
                            graphic[nextX, nextY,oppositeZ] = -1;

                            //领近节点(墙)是通路了，才可以递归
                            dfsGenMaze(oppositeX, oppositeY,oppositeZ,maxNodeNum);
                            //回溯

                            //如果所有节点找到了，回溯的时候直接return，不能再继续破坏墙，才能形成一个迷宫树，否则只是联通的图，并非是树
                            if (isSelectedNodeNums >= maxNodeNum) return;
                        }                      
                    }
                }               
            }
        }
    }

    /// <summary>
    /// 在游戏世界创建迷宫
    /// </summary>
    private void createMaze()
    {
        for(int i = 0; i < maxX; i++)
        {
            for(int j =0; j < maxY; j++)
            {
                for(int k = 0; k < maxZ; k++)
                    if (graphic[i, j, k] == 1)
                        GameObject.Instantiate(Cubeprefabs, new Vector3(i * 2, k * 2, j * 2), Quaternion.identity);
            }
        }
        //在-1层加个底
        for (int i = 0; i < maxX; i++)
        {
            for (int j = 0; j < maxY; j++)
            {
                var go = GameObject.Instantiate(Cubeprefabs, new Vector3(i, -1, j), Quaternion.identity);
                go.GetComponent<Renderer>().material.color = Color.gray;
            }
        }


    }

    /// <summary>
    /// 输出迷宫,用于Debug
    /// </summary>
    private void printMaze()
    {    
        for(int k = 0; k < maxZ; k++)
        {
            for (int i = 0; i < maxX; i++)
            {
                string output = "";
                for (int j = 0; j < maxY; j++)
                {
                    output += (graphic[i, j, k] + " ");
                }
                Debug.Log(output);
            }
        }   
       
    }
}
