using System.Collections;
using System.IO;
using UnityEngine;

namespace osgEx
{
    public class osgMono_LoadHelper : osgMono_Base
    {
        public string filePath;
        public GameObject loadedGameObject;
        Coroutine m_loadCorutine;
        public bool Load()
        {
            if (!string.IsNullOrWhiteSpace(filePath))
            {
                if (loadedGameObject)
                {
                    return true;
                }
                else if (m_loadCorutine == null)
                {
                    m_loadCorutine = StartCoroutine(coroutine_loading());
                }
            }
            return false;
        }
        public bool UnLoad()
        {
            if (!loadedGameObject)
            {
                if (m_loadCorutine != null)
                {
                    StopCoroutine(m_loadCorutine);
                    m_loadCorutine = null;
                }
            }
            Destroy(loadedGameObject);
            loadedGameObject = null;
            return true;
        }
        IEnumerator coroutine_loading()
        {
            gameObject.name = "loading_" + Path.GetFileName(filePath);
            var op = osg_Reader.LoadFromWebRequest(filePath);
            yield return op;
            if (op.osgReader != null)
            {
                gameObject.name = "loaded_" + Path.GetFileName(filePath);
                loadedGameObject = CreateGameObject(op.osgReader.root, gameObject);
            }
            else
            {
                gameObject.name = "error_" + Path.GetFileName(filePath);
                loadedGameObject = new GameObject();
                loadedGameObject.transform.parent = transform;
            }
            m_loadCorutine = null;
            yield break;
        }
        public static GameObject CreateGameObject(osg_Node node, GameObject parent = null)
        {
            GameObject osgbGameObject = new GameObject();
            Transform osgbTransform = osgbGameObject.transform;
            if (parent)
            {
                osgbTransform.parent = parent.transform;
                osgbTransform.localPosition = Vector3.zero;
                osgbTransform.localRotation = Quaternion.identity;
                osgbTransform.localScale = Vector3.one;
            }
            if (node is osg_Group group)
            {
                if (node is osg_MatrixTransform matrix)
                {
                    osgbTransform = osgbGameObject.transform;
                    osgbTransform.localPosition = matrix.localPosition;
                    osgbTransform.localRotation = matrix.localRotation;
                    osgbTransform.localScale = matrix.localScale;
                }
                foreach (var child in group.children)
                {
                    child.owner = node.owner;
                    CreateGameObject(child, osgbGameObject);
                }
                osgbGameObject.name = "OSGB_Group";
                return osgbGameObject;
            }

            if (node is osg_PagedLOD pagedLOD)
            {
                osgbGameObject.AddComponent<osgMono_PagedLOD>().Generate(pagedLOD);
                osgbGameObject.name = "OSGB_PagedLOD";
            }
            else if (node is osg_Geode osgGeode)
            {
                osgbGameObject.AddComponent<osgMono_Geode>().Generate(osgGeode);
                osgbGameObject.name = "OSGB_Geode";
            }
            return osgbGameObject;
        }

    }
}
