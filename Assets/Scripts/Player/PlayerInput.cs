using System.Collections;
using System.Collections.Generic;
using Template.Constant;
using UnityEngine;

namespace FourthTermPresentation
{
    public class PlayerInput : IInputPlayer
    {
        public float H => Input.GetAxisRaw(InputName.HORIZONTAL);
        public float V => Input.GetAxisRaw(InputName.VERTICAL);
        public bool Fire3 =>Input.GetButton(InputName.FIRE3);
    }
}
