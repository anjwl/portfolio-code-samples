// 실제 프로젝트에서 BFS 경로 탐색 부분만 발췌한 코드입니다.
// Enemy의 추상 메서드 구현과 이동, 충돌 처리는 생략했습니다.

public class MucusBunnySkeleton : Enemy
{
    const int MaxFindCount = 10201;

    private Stack<Vector2> backRoute = new Stack<Vector2>();
    private List<MiniRoute> miniRoutes = new List<MiniRoute>();
    private bool[] check = new bool[MaxFindCount + 1];
    private Queue<MiniRoute> findRouteQueue = new Queue<MiniRoute>();

    struct MiniRoute 
    {
        public MiniRoute(Vector3 position, int parentIndex, int index)
        {
            this.position = position;
            this.parentIndex = parentIndex;
            this.index = index;
        }
        public Vector3 position;
        public int parentIndex;
        public int index;
    }

    private void FindRoute() 
    {
        backRoute.Clear();
        miniRoutes.Clear();
        findRouteQueue.Clear();
        MiniRoute finalRoute = new MiniRoute(transform.position, -1, 0);
        miniRoutes.Add(finalRoute);
        findRouteQueue.Enqueue(finalRoute);

        for (int i = 0; i < check.Length; i++)
        {
            check[i] = false;
        }

        // EnemyUtil.MiniHash: 위치를 index와 1대1 대응시켜주는 함수
        check[EnemyUtil.MiniHash(transform, transform.position)] = true;

        int findCount = 0;
        Vector3 pos;
        while (true) 
        {
            if (findRouteQueue.Count == 0) 
            {
                Debug.LogError("경로를 찾을 수 없습니다.");
                backRoute.Clear();
                miniRoutes.Clear();
                return;
            }

            finalRoute = findRouteQueue.Dequeue();
            pos = finalRoute.position;

            // CheckOnRoute: 지정 경로에 해당 위치가 있는지 확인
            if (CheckOnRoute(new Vector2(pos.x, pos.z)) != -1) 
            {
                break;
            }

            if (findCount > MaxFindCount) 
            {
                Debug.LogError("경로를 찾을 수 없습니다.");
                backRoute.Clear();
                miniRoutes.Clear();
                return;
            }

            foreach (Vector3 dir in EnemyUtil.Dir) 
            {
                Vector3 newPos = pos + dir * 2;
                int hash = EnemyUtil.MiniHash(transform, newPos);

                if (Physics.Raycast(newPos + new Vector3(0, 5, 0), Vector3.down, 5, cantOn)) 
                {
                    check[hash] = true;
                    continue;
                }

                if (check[hash]) 
                {
                    continue;
                }

                MiniRoute route = new MiniRoute(newPos, finalRoute.index, miniRoutes.Count);
                miniRoutes.Add(route);
                findRouteQueue.Enqueue(route);
                check[hash] = true;
            }
            
            findCount++;
        }
        
        while (finalRoute.parentIndex != -1)
        {
            backRoute.Push(new Vector2(finalRoute.position.x, finalRoute.position.z));
            finalRoute = miniRoutes[finalRoute.parentIndex];
        }
        miniRoutes.Clear();
    }
}