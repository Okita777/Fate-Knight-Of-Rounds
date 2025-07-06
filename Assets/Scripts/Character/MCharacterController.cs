using System;
using System.Collections.Generic;
using okita.CharacterStateMachine;
using UnityEngine;

namespace okita.Character
{
    public class MCharacterController : MonoBehaviour
    {

        private Animator animator;
        [SerializeField] private StateMachine stateMachine;

        public Animator Animator
        {
            get => animator;
            private set => animator = value;
        }

        public StateMachine StateMachine
        {
            get => stateMachine;
            private set => stateMachine = value;
        }



        private void Awake()
        {

        }

        void Start()
        {

        }

        void Update()
        {

        }

        void LateUpdate()
        {

        }
    }
}


