using UnityEngine;

namespace Asset_Cleaner {
    public struct PrevClick {
        private const float DoubleClickTime = 0.5f;
        private Object _target;
        private float _timeClicked;

        public PrevClick(Object target) {
            _target = target;
            _timeClicked = Time.realtimeSinceStartup;
        }

        public bool IsDoubleClick(Object o) {
            return _target == o && Time.realtimeSinceStartup - _timeClicked < DoubleClickTime;
        }
    }
}