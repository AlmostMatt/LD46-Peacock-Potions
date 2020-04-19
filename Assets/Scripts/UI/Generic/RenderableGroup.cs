using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Instantiates and updates game objects based on a list of renderables.
public class RenderableGroup<RenderableType>
{
    private List<GameObject> mObjectPool = new List<GameObject>();
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
        child.transform.SetParent(null, false);
        // The child game object will act as a template for the renderables in this group.
        mNewObjectInstance = child;
        mRenderFunction = renderFunction;
        mOnCreateCallback = onCreateCallback;
    }

    // Should be called once per frame
    public void UpdateRenderables(List<RenderableType> renderables)
    {
        int index = 0;
        if (renderables != null)
        {
            foreach (RenderableType renderable in renderables)
            {
                GameObject renderObject;
                // Spawn new renderable
                if (index >= mContainer.childCount)
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
                    renderObject.transform.SetParent(mContainer, false);
                    if (mOnCreateCallback != null)
                    {
                        mOnCreateCallback(renderObject);
                    }
                }
                else
                {
                    renderObject = mContainer.GetChild(index).gameObject;
                }
                renderObject.SetActive(true);
                mRenderFunction(renderable, renderObject);
                index++;
            }
        }
        // Disable any excess children
        for (int i = index; i < mContainer.childCount; i++)
        {
            GameObject excessObject = mContainer.GetChild(i).gameObject;
            mObjectPool.Add(excessObject);
            excessObject.SetActive(false);
            excessObject.transform.SetParent(null, false);
        }
    }
}