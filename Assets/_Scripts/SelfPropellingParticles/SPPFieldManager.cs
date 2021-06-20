using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Serialization;
using Object = System.Object;

public class SPPFieldManager : MonoBehaviour
{
    [SerializeField] public SPPCell[] cells; // unimplemented

    [SerializeField] private int cellAmount;

    [SerializeField] private float xMin = -8f;
    [SerializeField] private float xMax = 8f;
    [SerializeField] private float yMin = -14.5f;
    [SerializeField] private float yMax = 14.5f;

    [SerializeField] private float velocity = 1f;
    [SerializeField] private float neighborhoodRadius = 5f;
    [SerializeField] private float fixedRotation = 180f;
    [SerializeField] private float perNeighborRotation = 17f;

    [SerializeField] private Color colorFree;
    [SerializeField] private Color color3_5;
    [SerializeField] private Color color6_9;
    [SerializeField] private Color color10_15;
    [SerializeField] private Color color16_25;
    [SerializeField] private Color color26_up;

    [SerializeField] private int framesPerListUpdate = 10;

    [SerializeField] private string cellParentName = "Cell Parent";

    private GameObject cellParent;

    private void Awake()
    {
        cellParent = GameObject.Find(cellParentName);

        cells = new SPPCell[0];

        managerName = gameObject.name;

        cells = cellParent.GetComponentsInChildren<SPPCell>();
    }


    [BurstCompile(Debug = false)]
    public struct NeighborJob : IJob, IDisposable
    {
        public Vector3 cellPosition;
        public NativeArray<Vector3> cellPositions;
        public float fixedCellRotation;
        public float neighborDistanceMagnitude;

        public int numberOfNeighbors;

        /* Represents the balance between left and right neighbors; negative number means there are more left members. */
        public int neighborCoefficient;
        public NativeArray<float> resultInfo;
        public float deltaRotation;
        public float perNeighborRotation;


        public NeighborJob(Vector3 cellPosition, NativeArray<Vector3> cellPositions, float fixedCellRotation,
            float perNeighborRotation, float neighborDistance) : this()
        {
            this.cellPosition = cellPosition;
            this.cellPositions = cellPositions;

            this.perNeighborRotation = perNeighborRotation;
            this.fixedCellRotation = fixedCellRotation;

            neighborDistanceMagnitude = neighborDistance * neighborDistance;
            resultInfo = new NativeArray<float>(2, Allocator.TempJob);
            numberOfNeighbors = 0;
            neighborCoefficient = 0;
            deltaRotation = 0;
        }


        public void Execute()
        {
            for (int i = 0; i < cellPositions.Length; i++)
            {
                if (cellPosition == cellPositions[i]) { continue; }

                var otherCellX = cellPositions[i].x;

                var diffX = otherCellX - cellPosition.x;
                var diffY = cellPositions[i].y - cellPosition.y;
                var distMag = (diffX * diffX) + (diffY * diffY);


                numberOfNeighbors += distMag <= neighborDistanceMagnitude ? 1 : 0;
                neighborCoefficient += otherCellX < cellPosition.x ? 1 : -1; // the cell 
            }

            deltaRotation = fixedCellRotation +
                            perNeighborRotation * numberOfNeighbors * Mathf.Sign(neighborCoefficient);

            resultInfo[0] = numberOfNeighbors - 1;
            resultInfo[1] = deltaRotation;
        }

        public void Dispose()
        {
            resultInfo.Dispose();
        }
    }


    private NeighborJob data;
    private List<JobHandle> _handles = new List<JobHandle>();

    private string managerName;


    private void LateUpdate()
    {
        cells = cellParent.GetComponentsInChildren<SPPCell>();
        // cells = FindObjectsOfType<SPPCell>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        var otherCellPositions =
            new NativeArray<Vector3>(cells.Length, Allocator.TempJob, NativeArrayOptions.ClearMemory);
        int cellArrayIndex = 0;

        foreach (var cell in cells)
        {
            if (cell == null) { continue; }

            otherCellPositions[cellArrayIndex] = cell.ThisTransform.position;
            cellArrayIndex++;
        }


        foreach (var cell in cells)
        {
            if (cell == null) { continue; }

            if (cell.ManagerName != managerName) { continue; }

            var cellTransform = cell.ThisTransform;
            var position = cellTransform.position;

            NeighborJob data = new NeighborJob(position, otherCellPositions, FixedRotation,
                perNeighborRotation, neighborhoodRadius);

            JobHandle handle = data.Schedule();

            handle.Complete();


            float neighborCount = data.resultInfo[0];
            float deltaRotation = data.resultInfo[1];


            data.Dispose();

            // effecting changes
            // rotating
            cellTransform.Rotate(0, 0, deltaRotation);

            var rotation = cellTransform.rotation;

            cell.currentNumberOfNeighbors = (int) neighborCount;

            cellTransform.SetPositionAndRotation(
                position + new Vector3(Mathf.Cos(rotation.eulerAngles.z), Mathf.Sin(rotation.eulerAngles.z), 0) *
                (Velocity * Time.deltaTime),
                rotation);

            // 'wrapping' position of cell to achieve toroidal space.
            var wrappedPosition = wrapPositionToSpace(cellTransform.position);

            cellTransform.SetPositionAndRotation(wrappedPosition, rotation);
            
            // separate the neighbors into left and right
            cell.Renderer.color = SetColor((int) neighborCount);
            SetCellGrade((int) neighborCount, cell);
        }

        otherCellPositions.Dispose();
    }

    private Color SetColor(int numberOfNeighbors)
    {
        int colorNumber = (int) Mathf.RoundToInt(numberOfNeighbors / 5);
        switch (colorNumber)
        {
            case 0:
                return colorFree;
            case 1:
                return color3_5;
            case 2:
                return color6_9;
            case 3:
                return color10_15;
            case 4:
                return color16_25;
            default:
                return color26_up;
        }
    }

    private void SetCellGrade(int numberOfNeighbors, SPPCell cell)
    {
        int colorNumber = (int) Mathf.RoundToInt(numberOfNeighbors / 5);
        switch (colorNumber)
        {
            case 0:
                cell.CellGrade = 0;
                break;
            case 1:
                cell.CellGrade = 1;
                break;
            case 2:
                cell.CellGrade = 2;
                break;
            case 3:
                cell.CellGrade = 3;
                break;
            case 4:
                cell.CellGrade = 4;
                break;
            case 5:
                cell.CellGrade = 5;
                break;
            default:
                cell.CellGrade = 5;
                break;
        }
    }

    private Vector3 wrapPositionToSpace(Vector3 position)
    {
        if (position.x > XMax) position.x = XMin;
        else if (position.x < XMin) position.x = XMax;
        if (position.y > YMax) position.y = YMin;
        else if (position.y < YMin) position.y = YMax;
        return position;
    }

    public SPPCell[] Cells
    {
        get => cells;
        set => cells = value;
    }

    public float XMin
    {
        get => xMin;
        set => xMin = value;
    }

    public float XMax
    {
        get => xMax;
        set => xMax = value;
    }

    public float YMin
    {
        get => yMin;
        set => yMin = value;
    }

    public float YMax
    {
        get => yMax;
        set => yMax = value;
    }


    public float Velocity
    {
        get => velocity;
        set => velocity = value;
    }

    public float NeighborhoodRadius
    {
        get => neighborhoodRadius;
        set => neighborhoodRadius = value;
    }

    public float FixedRotation
    {
        get => fixedRotation;
        set => fixedRotation = value;
    }

    public float PerNeighborRotation
    {
        get => perNeighborRotation;
        set => perNeighborRotation = value;
    }
}