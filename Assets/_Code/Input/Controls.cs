// GENERATED AUTOMATICALLY FROM 'Assets/Assets/GameInput.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace Shipwreck
{
    public class @GameInput : IInputActionCollection, IDisposable
    {
        public InputActionAsset asset { get; }
        public @GameInput()
        {
            asset = InputActionAsset.FromJson(@"{
    ""name"": ""GameInput"",
    ""maps"": [
        {
            ""name"": ""EvidenceBoard"",
            ""id"": ""c1b38be0-3d1e-4fb4-b48a-3174fb8577fb"",
            ""actions"": [
                {
                    ""name"": ""Interact"",
                    ""type"": ""Button"",
                    ""id"": ""1d1c2e72-7b2f-411e-a5b5-8deeb555a3fd"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Position"",
                    ""type"": ""Value"",
                    ""id"": ""a19c0665-0567-4bbc-bbb3-e50358024962"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""2e867fa6-18bc-446c-ac90-9a1d0fa6049c"",
                    ""path"": ""<Touchscreen>/press"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Interact"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""44792d8e-887b-49ba-86dd-bd2befa47f94"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Interact"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""fddaa4c3-7cca-471b-867a-93568aee2397"",
                    ""path"": ""<Touchscreen>/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Position"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""cd513118-14d6-486f-89a7-18cc26ba0944"",
                    ""path"": ""<Mouse>/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Position"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
            // EvidenceBoard
            m_EvidenceBoard = asset.FindActionMap("EvidenceBoard", throwIfNotFound: true);
            m_EvidenceBoard_Interact = m_EvidenceBoard.FindAction("Interact", throwIfNotFound: true);
            m_EvidenceBoard_Position = m_EvidenceBoard.FindAction("Position", throwIfNotFound: true);
        }

        public void Dispose()
        {
            UnityEngine.Object.Destroy(asset);
        }

        public InputBinding? bindingMask
        {
            get => asset.bindingMask;
            set => asset.bindingMask = value;
        }

        public ReadOnlyArray<InputDevice>? devices
        {
            get => asset.devices;
            set => asset.devices = value;
        }

        public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

        public bool Contains(InputAction action)
        {
            return asset.Contains(action);
        }

        public IEnumerator<InputAction> GetEnumerator()
        {
            return asset.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Enable()
        {
            asset.Enable();
        }

        public void Disable()
        {
            asset.Disable();
        }

        // EvidenceBoard
        private readonly InputActionMap m_EvidenceBoard;
        private IEvidenceBoardActions m_EvidenceBoardActionsCallbackInterface;
        private readonly InputAction m_EvidenceBoard_Interact;
        private readonly InputAction m_EvidenceBoard_Position;
        public struct EvidenceBoardActions
        {
            private @GameInput m_Wrapper;
            public EvidenceBoardActions(@GameInput wrapper) { m_Wrapper = wrapper; }
            public InputAction @Interact => m_Wrapper.m_EvidenceBoard_Interact;
            public InputAction @Position => m_Wrapper.m_EvidenceBoard_Position;
            public InputActionMap Get() { return m_Wrapper.m_EvidenceBoard; }
            public void Enable() { Get().Enable(); }
            public void Disable() { Get().Disable(); }
            public bool enabled => Get().enabled;
            public static implicit operator InputActionMap(EvidenceBoardActions set) { return set.Get(); }
            public void SetCallbacks(IEvidenceBoardActions instance)
            {
                if (m_Wrapper.m_EvidenceBoardActionsCallbackInterface != null)
                {
                    @Interact.started -= m_Wrapper.m_EvidenceBoardActionsCallbackInterface.OnInteract;
                    @Interact.performed -= m_Wrapper.m_EvidenceBoardActionsCallbackInterface.OnInteract;
                    @Interact.canceled -= m_Wrapper.m_EvidenceBoardActionsCallbackInterface.OnInteract;
                    @Position.started -= m_Wrapper.m_EvidenceBoardActionsCallbackInterface.OnPosition;
                    @Position.performed -= m_Wrapper.m_EvidenceBoardActionsCallbackInterface.OnPosition;
                    @Position.canceled -= m_Wrapper.m_EvidenceBoardActionsCallbackInterface.OnPosition;
                }
                m_Wrapper.m_EvidenceBoardActionsCallbackInterface = instance;
                if (instance != null)
                {
                    @Interact.started += instance.OnInteract;
                    @Interact.performed += instance.OnInteract;
                    @Interact.canceled += instance.OnInteract;
                    @Position.started += instance.OnPosition;
                    @Position.performed += instance.OnPosition;
                    @Position.canceled += instance.OnPosition;
                }
            }
        }
        public EvidenceBoardActions @EvidenceBoard => new EvidenceBoardActions(this);
        public interface IEvidenceBoardActions
        {
            void OnInteract(InputAction.CallbackContext context);
            void OnPosition(InputAction.CallbackContext context);
        }
    }
}
