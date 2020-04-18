using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Instantiates and updates game objects based on a list of renderables.
public class RenderableGroup<RenderableType>
{
    private List<GameObject> mObjectPool = new List<GameObject>();
    private BidirectionalMap<RenderableType, GameObject> mRenderableObjectMap = new BidirectionalMap<RenderableType, GameObject>();
    private Transform mContainer;
    private GameObject mNewObjectInstance;
    private System.Action<RenderableType, GameObject> mRenderFunction;
    private System.Action<GameObject> mOnCreateCallback;

    // Creates a renderable group based on a container, an object instance, and a render function.
    public RenderableGroup(
        Transform container,
        GameObject newObjectInstance,
        System.Action<RenderableType, GameObject> renderFunction,
        System.Action<GameObject> onCreateCallback = null)
    {
        mContainer = container;
        mNewObjectInstance = newObjectInstance;
        mRenderFunction = renderFunction;
        mOnCreateCallback = onCreateCallback;
    }

    // Creates a renderable group based on a container, assuming that the container has exactly one child that should be used as a new-object instance
    public RenderableGroup(
        Transform container,
        System.Action<RenderableType, GameObject> renderFunction,
        System.Action<GameObject> onCreateCallback = null)
    {
        mContainer = container;
        // Disable and detach the child, we just want to be able to copy it later
        GameObject child = container.GetChild(0).gameObject;
        child.SetActive(false);
        child.transform.SetParent(null);
        // The child game object will act as a template for the renderables in this group.
        mNewObjectInstance = child;
        mRenderFunction = renderFunction;
        mOnCreateCallback = onCreateCallback;
    }

    // Should be called once per frame
    public void UpdateRenderables(List<RenderableType> renderables)
    {
        // Put all GameObjects in a set, and remove the objects that are still in use
        HashSet<GameObject> unusedObjects = new HashSet<GameObject>(mRenderableObjectMap.Values);
        if (renderables != null)
        {
            foreach (RenderableType renderable in renderables)
            {
                GameObject renderObject;
                // Spawn new renderable
                if (!mRenderableObjectMap.ContainsKey(renderable))
                {
                    if (mObjectPool.Count > 0)
                    {
                        renderObject = mObjectPool[mObjectPool.Count - 1];
                        mObjectPool.RemoveAt(mObjectPool.Count - 1);
                    }
                    else
                    {
                        renderObject = GameObject.Instantiate(mNewObjectInstance);
                    }
                    renderObject.SetActive(true);
                    renderObject.transform.SetParent(mContainer, false);
                    mRenderableObjectMap.Add(renderable, renderObject);
                    if (mOnCreateCallback != null)
                    {
                        mOnCreateCallback(renderObject);
                    }
                }
                else
                {
                    renderObject = mRenderableObjectMap.GetValue(renderable);
                    unusedObjects.Remove(renderObject);
                }
                mRenderFunction(renderable, renderObject);
            }
        }
        foreach (GameObject renderObject in unusedObjects)
        {
            mRenderableObjectMap.RemoveValue(renderObject);
            mObjectPool.Add(renderObject);
            renderObject.SetActive(false);
            renderObject.transform.SetParent(null, false);
        }
    }

    // Get the Renderable that was rendered to a GameObject.
    public RenderableType GetRenderableFromGameObject(GameObject renderObject)
    {
        return mRenderableObjectMap.GetKey(renderObject);
    }

    // Get the GameObject to which a Renderable was rendered.
    public GameObject GetGameObjectFromRenderable(RenderableType renderable)
    {
        return mRenderableObjectMap.GetValue(renderable);
    }

    public Dictionary<RenderableType, GameObject>.ValueCollection GetGameObjects()
    {
        return mRenderableObjectMap.Values;
    }
}