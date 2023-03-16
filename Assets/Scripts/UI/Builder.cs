using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Nanoreno.UI.Builder
{
    public abstract class Builder
    {
        protected VisualTreeAsset treeAsset;

        public Builder(VisualTreeAsset asset)
        {
            treeAsset = asset;
        }

        public abstract VisualElement Build();
    }
}
