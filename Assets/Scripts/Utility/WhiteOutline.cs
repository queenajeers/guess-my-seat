using UnityEngine.Pool;

namespace UnityEngine.UI
{
    [AddComponentMenu("UI/Effects/True White Outline", 81)]
    public class WhiteOutline : Shadow
    {
        protected WhiteOutline() { }

        public override void ModifyMesh(VertexHelper vh)
        {
            if (!IsActive())
                return;

            var verts = ListPool<UIVertex>.Get();
            vh.GetUIVertexStream(verts);

            var neededCapacity = verts.Count * 5;
            if (verts.Capacity < neededCapacity)
                verts.Capacity = neededCapacity;

            // Always white outline (with original alpha from effectColor)
            byte alpha = (byte)(effectColor.a * 255f);
            Color32 white = new Color32(255, 255, 255, alpha);

            int start = 0;
            int end = verts.Count;

            // Apply shadow in all directions
            ApplySolidWhite(verts, white, start, verts.Count, effectDistance.x, effectDistance.y);
            start = end; end = verts.Count;
            ApplySolidWhite(verts, white, start, verts.Count, effectDistance.x, -effectDistance.y);
            start = end; end = verts.Count;
            ApplySolidWhite(verts, white, start, verts.Count, -effectDistance.x, effectDistance.y);
            start = end; end = verts.Count;
            ApplySolidWhite(verts, white, start, verts.Count, -effectDistance.x, -effectDistance.y);

            // Clear the old mesh and set the new one
            vh.Clear();
            vh.AddUIVertexTriangleStream(verts);
            ListPool<UIVertex>.Release(verts);
        }

        void ApplySolidWhite(System.Collections.Generic.List<UIVertex> verts, Color32 color, int start, int end, float x, float y)
        {
            for (int i = start; i < end; ++i)
            {
                var vt = verts[i];
                vt.position.x += x;
                vt.position.y += y;

                // Set the outline color to white
                vt.color = color;

                // Set invalid UVs to ensure it doesn't sample the original texture
                vt.uv0 = new Vector2(-999, -999);  // Invalid UVs to avoid texture sampling

                verts.Add(vt);  // Add the modified vertex
            }
        }
    }
}
