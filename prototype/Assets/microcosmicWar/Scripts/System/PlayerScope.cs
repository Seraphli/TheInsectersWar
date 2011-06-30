using UnityEngine;

public class PlayerScope:MonoBehaviour
{
    //public zzCharacter character;
    public ActionCommandControl actionCommandControl;
    public Transform playerTransform;
    public BoundNetworkScope boundNetworkScope;
    public float updateScopeInterval = 0.0667f;

    [System.Serializable]
    public class ScopeRect
    {
        public ScopeRect() { }
        public ScopeRect(ScopeRect pOther)
        {
            left = pOther.left;
            right = pOther.right;
            top = pOther.top;
            bottom = pOther.bottom;
        }

        public float left;
        public float right;
        public float top;
        public float bottom;
    }

    public ScopeRect edgeWhenStatic;
    public ScopeRect edgeWhenMove;

    public LayerMask groundLayerMask;
    public float maxGroundDistanceCheck;
    public float edgeForGround;

    void Start()
    {
        if (!Network.isServer || boundNetworkScope.networkPlayer == Network.player)
        {
            Destroy(this);
            Destroy(boundNetworkScope);
            return;
        }
        var lTimer = gameObject.AddComponent<zzTimer>();
        lTimer.setInterval(updateScopeInterval);
        lTimer.addImpFunction(updateScope);
        var lSceneManager = GameSceneManager.Singleton;
        boundNetworkScope.networkPlayerRoots
            = new Transform[]{
                lSceneManager.getManager(Race.ePismire,
                    GameSceneManager.UnitManagerType.soldier).managerRoot,
                lSceneManager.getManager(Race.eBee,
                    GameSceneManager.UnitManagerType.soldier).managerRoot,
            };
    }

    void updateScope()
    {
        if (playerTransform)
            _playerScope = calculatePlayerScope();
        boundNetworkScope.updateScope(_playerScope);
    }

    public Bounds calculatePlayerScope()
    {
        Vector3 lHeroPosition = playerTransform.position;
        ScopeRect lScopeEdge = new ScopeRect(edgeWhenStatic);
        var lCommand = actionCommandControl.getCommand();
        if (lCommand.GoForward)
        {
            if (actionCommandControl.face == UnitFaceDirection.left)
                lScopeEdge.left = edgeWhenMove.left;
            else
                lScopeEdge.right = edgeWhenMove.right;
        }

        RaycastHit lRaycastHit;
        if (Physics.Raycast(lHeroPosition, Vector3.down, out lRaycastHit,
            maxGroundDistanceCheck, groundLayerMask))
        {
            lScopeEdge.bottom = lHeroPosition.y - lRaycastHit.point.y + edgeForGround;
        }
        else
        {
            lScopeEdge.bottom = maxGroundDistanceCheck + edgeForGround;
        }

        Vector3 lBoundCenter =
            new Vector3(lHeroPosition.x + (lScopeEdge.right - lScopeEdge.left) / 2f,
                lHeroPosition.y + (lScopeEdge.top - lScopeEdge.bottom) / 2f,
                0f);
        return new Bounds(lBoundCenter,
            new Vector3(lScopeEdge.left + lScopeEdge.right,
                lScopeEdge.top + lScopeEdge.bottom, 1f)
                );
    }

    [SerializeField]
    Bounds _playerScope;

    //Bounds playerScope
    //{
    //    set { _playerScope = value; }
    //}

    void OnDrawGizmosSelected()
    {
        if (playerTransform)
        {
            if (!enabled)
                _playerScope = calculatePlayerScope();
            Gizmos.color = Color.green;
            var lPlayerScopeMin = _playerScope.min;
            lPlayerScopeMin.z = 0f;
            var lPlayerScopeMax = _playerScope.max;
            lPlayerScopeMax.z = 0f;
            var lLeftTop = new Vector3(lPlayerScopeMin.x, lPlayerScopeMax.y);
            var lRightBottom = new Vector3(lPlayerScopeMax.x, lPlayerScopeMin.y);

            Gizmos.DrawLine(lPlayerScopeMin, lLeftTop);
            Gizmos.DrawLine(lLeftTop, lPlayerScopeMax);
            Gizmos.DrawLine(lPlayerScopeMax, lRightBottom);
            Gizmos.DrawLine(lRightBottom, lPlayerScopeMin);

        }
    }
}