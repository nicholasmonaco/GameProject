using System;
using System.Collections.Generic;
using System.Text;

namespace GameProject.Code.Core.UI {
    public class UIComponent : Component {

        public RectTransform rectTransform;

        public UIComponent(GameObject attached) {
            gameObject = attached;

            if(!(attached.transform is RectTransform)) {
                gameObject.transform = gameObject.SwitchToRectTransform();
            }

            if (attached.transform.UIParentFlag == true) {
                attached.transform.Parent = GameManager.MainCanvas.transform;
            }

            transform = gameObject.transform;
            rectTransform = transform as RectTransform;
        }
    }
}
