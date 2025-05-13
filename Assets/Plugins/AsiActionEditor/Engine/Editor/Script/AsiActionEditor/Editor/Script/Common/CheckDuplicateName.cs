using System;
using System.Collections.Generic;
using System.Linq;

namespace AsiActionEngine.Editor
{
    public class CheckDuplicateName
    {
        #region Property
        struct DuplicateName
        {
            public string Name;
            public int DuplicateCount;

            public DuplicateName(string _name, int _DuplicateCount)
            {
                Name = _name;
                DuplicateCount = _DuplicateCount;
            }
        }
        private List<string> mNames;
        private Dictionary<string, DuplicateName> mDuplicateDic;
        private bool isIsDuplicate = false;

        #endregion
        
        public CheckDuplicateName()
        {
            mNames = new List<string>();
            mDuplicateDic = new Dictionary<string, DuplicateName>();
            isIsDuplicate = false;
        }
        
        public bool OnCheck(string _name, string _alternateName = "", Action<string> _uplicateName = null)
        {
            if (mNames.Contains(_name))
            {
                string _mainName = string.IsNullOrEmpty(_alternateName) ? _name : _alternateName;
                isIsDuplicate = true;
                if (mDuplicateDic.ContainsKey(_name))
                {
                    DuplicateName _newValue = mDuplicateDic[_name];
                    _newValue.DuplicateCount++;
                    mDuplicateDic[_name] = _newValue;
                }
                else
                {
                    DuplicateName _DuplicateCount = new DuplicateName(_mainName, 2);
                    mDuplicateDic.Add(_name,_DuplicateCount);
                }
                if (_uplicateName != null) _uplicateName(_name);
                return false;
            }
            else
            {
                mNames.Add(_name);
            }
            return true;
        }

        public bool IsDuplicate()
        {
            return isIsDuplicate;
        }

        public void OnClear()
        {
            mNames.Clear();
            mDuplicateDic.Clear();
            isIsDuplicate = false;
        }

        public string GetDuplicateName()
        {
            string _outName = string.Empty;
            foreach (var VARIABLE in mDuplicateDic)
            {
                //<color=#FFF100>{mEventList.count}</color>
                _outName += $"{VARIABLE.Key}:  {VARIABLE.Value.DuplicateCount}\n";
            }
            return _outName;
        }
    }
}