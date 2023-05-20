using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreatePlatforms : MonoBehaviour
{
    [SerializeField] public GameObject linePrefab;
    [SerializeField] private ParticleSystem paintParticles;
    [SerializeField] private Texture paintTexture;
    [SerializeField] private LayerMask stopPlatformLayerMask;

    private GameObject currentLine;

    private LineRenderer lineRenderer;
    private EdgeCollider2D edgeCollider;

    private List<Vector2> mousePositions = new List<Vector2>();
    private int numPlatforms = 0;

    private GameObject oldestPlatform;
    private GameObject middlestPlatform;
    private GameObject newestPlatform;

    public bool stopPlatform = false;
    public bool createPlatform = false; //acts as trigger to call CreateLine()
    private bool canCreatePlatform = false; //updates every frame - tells whether you can draw platform where mouse currently is
    private bool isDrawing = false; //tells whether a platform is being drawn. Set to true when CreateLine() called, set to false when canCreatePlatform is false
    private Vector2 prevPoint;

    float mouseDownTime = 0;
    Vector2 mouseDownPosition;

    [SerializeField] private SpriteMask drip;
    private bool PAINT_SFX = false;

    void Start()
    {
        prevPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        SceneManager.activeSceneChanged += ClearPlatforms;
    }

    void Update()
    {

        if (oldestPlatform != null)
        {
            Color redColor = new Color(1.0f, 0.0f, 0.0f);
            oldestPlatform.GetComponent<Renderer>().material.SetColor("_Color", redColor);
        }
        if (middlestPlatform != null)
        {
            Color orangeColor = new Color(1.0f, 0.64f, 0.0f);
            middlestPlatform.GetComponent<Renderer>().material.SetColor("_Color", orangeColor);
        }
        if (newestPlatform != null)
        {
            Color yellowPlatform = new Color(1.0f, 0.92f, 0.016f);
            newestPlatform.GetComponent<Renderer>().material.SetColor("_Color", yellowPlatform);
        }

        Vector2 tempMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D raycastHit = Physics2D.Linecast(prevPoint, tempMousePos, stopPlatformLayerMask);
        canCreatePlatform = true;
        if (raycastHit.collider != null)
        {
            canCreatePlatform = false;
        }
        prevPoint = tempMousePos;

        if (GameObject.Find("Paintbrush") != null && GameObject.Find("Paintbrush").GetComponent<Paintbrush>().brushState == Paintbrush.BrushState.inHand)
        {
            if (Input.GetMouseButtonDown(0))
            {
                mouseDownTime = Time.time;
                mouseDownPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            }
            else if (Input.GetMouseButton(0))
            {
                if (Time.time - mouseDownTime > 0.05f && Vector2.Distance(mouseDownPosition, Camera.main.ScreenToWorldPoint(Input.mousePosition)) > 0.05f && mouseDownTime != -1)
                {
                    mouseDownTime = -1;
                    createPlatform = true;
                    numPlatforms++;
                    CreateLine(mouseDownPosition);
                }
                
                if (createPlatform && canCreatePlatform)
                {
                    createPlatform = false;
                    isDrawing = true;
                }
                else if (createPlatform && !canCreatePlatform)
                {
                    isDrawing = true;
                }

                if (isDrawing)
                {
                    if (Vector2.Distance(tempMousePos, mousePositions[mousePositions.Count - 1]) > 0.1f && isDrawing)
                    {
                        if (canCreatePlatform)
                        {
                            UpdateLine(tempMousePos);
                        }
                        else
                        {
                            UpdateLine(raycastHit.point);
                            isDrawing = false;
                        }
                    }
                }


            }
            else if (Input.GetMouseButtonUp(0))
            {
                AudioManager.Instance.sfxSource.Stop();
                isDrawing = false;
                createPlatform = false;
                paintParticles.Stop();
            }
        }
    }

    void CreateLine(Vector2 mousePos)
    {
        paintParticles.Play();
        currentLine = Instantiate(linePrefab, Vector3.zero, Quaternion.identity);
        ObjectManager.Instance.AddDrawnPlatform(currentLine);
        lineRenderer = currentLine.GetComponent<LineRenderer>();
        lineRenderer.material.SetTexture("_MainTex", paintTexture);
        edgeCollider = currentLine.GetComponent<EdgeCollider2D>();
        mousePositions.Clear();
        mousePositions.Add(mousePos);
        mousePositions.Add(mousePos);
        lineRenderer.SetPosition(0, mousePositions[0]);
        lineRenderer.SetPosition(1, mousePositions[1]);
        edgeCollider.points = mousePositions.ToArray();

        if (numPlatforms == 1)
        {
            newestPlatform = currentLine;
        }
        else if (numPlatforms == 2)
        {
            middlestPlatform = newestPlatform;
            newestPlatform = currentLine;
        }
        else if (numPlatforms == 3)
        {
            oldestPlatform = middlestPlatform; 
            middlestPlatform = newestPlatform;
            newestPlatform = currentLine;
        }
        else
        {
            GameObject temp = oldestPlatform;
            oldestPlatform = middlestPlatform;
            middlestPlatform = newestPlatform;
            newestPlatform = currentLine;
            ObjectManager.Instance.RemoveDrawnPlatform(temp);
            Destroy(temp);
        }
    }

    void UpdateLine(Vector2 newMousePos)
    {
        //Debug.Log("update line");
        if (lineRenderer != null)
        {
            mousePositions.Add(newMousePos);
            Instantiate(drip, new Vector3(newMousePos.x, newMousePos.y, -1f), Quaternion.identity);
            lineRenderer.positionCount++;
            lineRenderer.SetPosition(lineRenderer.positionCount - 1, newMousePos);
            edgeCollider.points = mousePositions.ToArray();
            paintParticles.transform.position = newMousePos;

            if (!PAINT_SFX)
            {
                StartCoroutine(PaintingSFX());
            }
        }
    }

    void ClearPlatforms(Scene scene, Scene scene1)
    {
        if (oldestPlatform) Destroy(oldestPlatform);
        if (middlestPlatform) Destroy(middlestPlatform);
        if (newestPlatform) Destroy(newestPlatform);

        if (scene1.name == "Menu" || scene1.name == "Credits" || scene1.name == "BaseGame")
        {
            enabled = false;
        }
        else
        {
            enabled = true;
        }
    }

    public IEnumerator PaintingSFX()
    {
        PAINT_SFX = true;
        AudioManager.Instance.PlaySFX("Paintbrush_SFX");
        yield return new WaitForSeconds(0.2f);
        PAINT_SFX = false;
    }
}
