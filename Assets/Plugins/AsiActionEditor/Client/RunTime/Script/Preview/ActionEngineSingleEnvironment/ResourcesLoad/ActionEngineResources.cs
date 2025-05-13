// using UnityEditor.iOS;
using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AsiActionEngine.RunTime;
using UnityEditor;
using UnityEngine.Pool;
using Object = UnityEngine.Object;

// using Unity.VisualScripting;

namespace AsiTimeLine.RunTime
{
    public class ActionEngineResources
    {
        #region Instance

        private static ActionEngineResources _instance;

        public static ActionEngineResources Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ActionEngineResources();
                }
                //创建对象池父级便于管理
                GameObject _objectPoolParent = GameObject.Find("/ActionObjectPool");
                if (_objectPoolParent == null)
                {
                    _objectPoolParent = new GameObject();
                    _objectPoolParent.name = "ActionObjectPool";
                }
                m_ObjectPoolParent = _objectPoolParent.transform;
                
                return _instance;
            }
        }
        

        #endregion

        #region MyStruct

        private struct AutoDestory
        {
            public Component m_target;
            public ObjectPool<Component> m_objectPool;

            public AutoDestory(Component target,ObjectPool<Component> objectPool)
            {
                m_target = target;
                m_objectPool = objectPool;
            }
        }


        #endregion

        private Dictionary<string, UnityEngine.Object> m_AllObject = new Dictionary<string, UnityEngine.Object>();

        private Dictionary<string, List<Action<Object>>> m_AllObject_callback = new Dictionary<string, List<Action<Object>>>();
        private Dictionary<string, ObjectPool<Component>> m_ObjectPool = new Dictionary<string, ObjectPool<Component>>();
        private Dictionary<Component, AutoDestory> m_ConDestorData = new Dictionary<Component, AutoDestory>();
        private Dictionary<AutoDestory, float> m_AutoDestoryList = new Dictionary<AutoDestory, float>();
        private List<AutoDestory> m_AutoDestoryListKey = new List<AutoDestory>();
        private static Transform m_ObjectPoolParent = null;
        private struct LoadCallBack
        {
            public ResourceRequest RQ;
            public Action<UnityEngine.Object> CallBack;
            public LoadCallBack(ResourceRequest _Rq, Action<UnityEngine.Object> _callback)
            {
                RQ = _Rq;
                CallBack = _callback;
            }
        }

        private List<LoadCallBack> mAllCallBacks = new List<LoadCallBack>();
        
        public void LoadAsync(string _path, Action<UnityEngine.Object> _loadCallBack)
        {
            // ResourceRequest _rq = Resources.LoadAsync<GameObject>(_path);
            // mAllCallBacks.Add(new LoadCallBack(_rq, _loadCallBack));
            if (m_AllObject.TryGetValue(_path, out UnityEngine.Object _obj))
            {
                _loadCallBack(_obj);
            }
            else
            {
                // if (!m_AllObject_callback.ContainsKey(_path))
                //     m_AllObject_callback.Add(_path, new List<Action<Object>>());
                // m_AllObject_callback[_path].Add(_loadCallBack);
                
                ActionEnginLoadData.Instance.LoadObject<UnityEngine.Object>(_path, (_obj) =>
                {
                    _loadCallBack(_obj);
                    // foreach (Action<Object> VARIABLE in m_AllObject_callback[_path])
                    // {
                    //     VARIABLE(_obj);
                    // }
                    // m_AllObject_callback.Remove(_path);
                    
                    if (!m_AllObject.TryAdd(_path, _obj))
                    {
                        // Debug.LogWarning("重复加载 " + _path);
                    }
                });
            }
        }//避免重复加载同一个路径

        /// <summary>
        /// 加载路径下的对象并自动生成对象池
        /// </summary>
        /// <param name="_path">对象路径</param>
        /// <param name="_callback">异步加载回调</param>
        /// <param name="_onDestoryTime">对象自动回收的时间，小于等于零则不会自动回收</param>
        /// <param name="_maxConst">对象 池最大数量</param>
        public void LoadToObjectPool<T>(string _path, Action<UnityEngine.Component> _callback, float _onDestoryTime = -1,
            int _maxConst = 10) where T : Component
        {
            if(string.IsNullOrEmpty(_path))return;
            if (m_ObjectPool.TryGetValue(_path, out ObjectPool<UnityEngine.Component> _objectPool))
            {
                UnityEngine.Component _component = _objectPool.Get();
                _callback(_component);
                if (_onDestoryTime > 0)
                {
                    AutoDestory _autoDestory = new AutoDestory(_component, _objectPool);
                    m_AutoDestoryList.Add(_autoDestory, _onDestoryTime);
                    m_ConDestorData.Add(_component, _autoDestory);
                }
            }
            else
            {
                //异步加载
                LoadAsync(_path, (Object _obj) =>
                {
                    if (_obj is null)
                    {
                        EngineDebug.LogError($"事件对象加载路径错误\n<color=#FFCC00>{_path}</color>");
                        return;
                    }

                    if (_obj is GameObject _gameObject)
                    {
                        ObjectPool<Component> _monsterPool = new ObjectPool<Component>(
                            //新建对象
                            () =>
                            {
                                #if UNITY_EDITOR
                                if (!Object.Instantiate(_gameObject, m_ObjectPoolParent).TryGetComponent(out T aa))
                                {
                                    // UnityEditor.EditorUtility.DisplayDialog("警告", $"当前尝试请求 【{typeof(T).Name}】 组件，但该对象不具有这样的组件", "OK");
                                    EngineDebug.LogError($"当前尝试请求 【{typeof(T).Name}】 组件，但该对象不具有这样的组件\n <color=#FFCC00>{_path}</color>");
                                    return null;
                                }
                                #endif
                                return Object.Instantiate(_gameObject, m_ObjectPoolParent).GetComponent<T>();
                            },
                            //取出对象
                            (UnityEngine.Component _object) => { _object.gameObject.SetActive(true); },
                            //存入对象
                            (UnityEngine.Component _object) => { _object.gameObject.SetActive(false); },
                            //销毁对象
                            (UnityEngine.Component _object) => { Object.Destroy(_object.gameObject); },
                            true,
                            3,
                            _maxConst
                        );
                        m_ObjectPool.Add(_path, _monsterPool);
                        UnityEngine.Component _component = _monsterPool.Get();
                        _callback(_component);
                        if (_onDestoryTime > 0)
                        {
                            AutoDestory _autoDestory = new AutoDestory(_component, _monsterPool);
                            m_AutoDestoryList.Add(_autoDestory, _onDestoryTime);
                            m_ConDestorData.Add(_component, _autoDestory);
                        }
                    }
                });
            }
        } //从对象池加载

        public bool Remove(string _path, UnityEngine.Component _target)
        {
            if (m_ObjectPool.TryGetValue(_path, out ObjectPool<Component> _objectPool))
            {
                _objectPool.Release(_target);
                return true;
            }
            return false;
        }

        public bool ResetLife(UnityEngine.Component _target, float _life)
        {
            if (m_ConDestorData.TryGetValue(_target, out AutoDestory _autoDestory))
            {
                m_AutoDestoryList[_autoDestory] = _life;
                return true;
            }
            #if UNITY_EDITOR
            EngineDebug.LogWarning("重置生命失败");
            #endif
            return false;
        }
        public void ChackCallBack(float _deltaTime)
        {
            for (int i = 0; i < mAllCallBacks.Count; i++)
            {
                if (mAllCallBacks[i].RQ.isDone)
                {
                    if (mAllCallBacks[i].RQ.asset is GameObject _gameObject)
                    {
                        if (mAllCallBacks[i].CallBack is not null)
                            mAllCallBacks[i].CallBack(mAllCallBacks[i].RQ.asset);
                        else
                            EngineDebug.LogWarning($"加载回调是空的");
                    }
                    else
                    {
                        mAllCallBacks[i].CallBack(null);
                    }
                    mAllCallBacks.RemoveAt(i);
                }
            }

            m_AutoDestoryListKey = m_AutoDestoryList.Keys.ToList();
            foreach (var VARIABLE in m_AutoDestoryListKey)
            {
                m_AutoDestoryList[VARIABLE] -= _deltaTime;
                if (m_AutoDestoryList[VARIABLE] < 0)
                {
                    VARIABLE.m_objectPool.Release(VARIABLE.m_target);
                    m_AutoDestoryList.Remove(VARIABLE);
                    m_ConDestorData.Remove(VARIABLE.m_target);
                }
            }
        }

        public string GetAssetPath(UnityEngine.Object _obj)
        {
#if UNITY_EDITOR
            return UnityEditor.AssetDatabase.GetAssetPath(_obj);
#endif
            return string.Empty;
        }
    }
}