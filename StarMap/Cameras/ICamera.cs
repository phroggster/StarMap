using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarMap.Cameras
{
    public interface ICamera
    {
        #region --- Properties ---

        bool IsUserMovable { get; }
        string Name { get; }
        Quaternion Orientation { get; }
        Vector3 Position { get; }

        #endregion // --- Properties ---


        #region --- Methods ---

        void AbortLerp();

        void BeginLerp(Vector3 position, float speed, Quaternion orientation);

        void BindViewMatrix(int uniform);

        void LookAt(Vector3 target);

        void Move(Vector3 offset);

        void MoveTo(Vector3 position);

        void Rotate(Quaternion rotation);

        void RotateTo(Quaternion orientation);

        void Update(double delta);

        #endregion // --- Methods ---
    }
}
