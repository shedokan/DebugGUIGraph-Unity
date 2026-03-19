using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace WeavUtils.Tests
{
    // MonoBehaviours for supported types
    public class SupportedFloatBehaviour : MonoBehaviour
    {
        [DebugGUIGraph] public float val = 1f;
    }

    public class SupportedIntBehaviour : MonoBehaviour
    {
        [DebugGUIGraph] public int val = 1;
    }

    public class SupportedDoubleBehaviour : MonoBehaviour
    {
        [DebugGUIGraph] public double val = 1.0;
    }

    public class SupportedByteBehaviour : MonoBehaviour
    {
        [DebugGUIGraph] public byte val = 1;
    }

    public class SupportedShortBehaviour : MonoBehaviour
    {
        [DebugGUIGraph] public short val = 1;
    }

    public class SupportedLongBehaviour : MonoBehaviour
    {
        [DebugGUIGraph] public long val = 1;
    }

    public class SupportedBoolBehaviour : MonoBehaviour
    {
        [DebugGUIGraph] public bool val = true;
    }

    public class UnsupportedStringBehaviour : MonoBehaviour
    {
        [DebugGUIGraph] public string val = "hello";
    }

    // Property test behaviours
    public class SupportedFloatPropertyBehaviour : MonoBehaviour
    {
        [DebugGUIGraph] public float Val => 1f;
    }

    public class SupportedIntPropertyBehaviour : MonoBehaviour
    {
        [DebugGUIGraph] public int Val => 1;
    }

    public class UnsupportedStringPropertyBehaviour : MonoBehaviour
    {
        [DebugGUIGraph] public string Val => "hello";
    }

    public class TypeAcceptanceTests
    {
        GameObject testObject;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            // Ensure DebugGUI singleton is initialized
            yield return null;
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            if (testObject != null)
                Object.Destroy(testObject);
            DebugGUI.ForceReinitializeAttributes();
            yield return null;
        }

        void CreateWithComponent<T>() where T : Component
        {
            testObject = new GameObject($"TypeTest_{typeof(T).Name}");
            testObject.AddComponent<T>();
            DebugGUI.ForceReinitializeAttributes();
        }

        // ── Supported field types ────────────────────────────────────────

        [UnityTest]
        public IEnumerator Float_Field_IsSupported()
        {
            CreateWithComponent<SupportedFloatBehaviour>();
            LogAssert.NoUnexpectedReceived();
            yield return null;
        }

        [UnityTest]
        public IEnumerator Int_Field_IsSupported()
        {
            CreateWithComponent<SupportedIntBehaviour>();
            LogAssert.NoUnexpectedReceived();
            yield return null;
        }

        [UnityTest]
        public IEnumerator Double_Field_IsSupported()
        {
            CreateWithComponent<SupportedDoubleBehaviour>();
            LogAssert.NoUnexpectedReceived();
            yield return null;
        }

        [UnityTest]
        public IEnumerator Byte_Field_IsSupported()
        {
            CreateWithComponent<SupportedByteBehaviour>();
            LogAssert.NoUnexpectedReceived();
            yield return null;
        }

        [UnityTest]
        public IEnumerator Short_Field_IsSupported()
        {
            CreateWithComponent<SupportedShortBehaviour>();
            LogAssert.NoUnexpectedReceived();
            yield return null;
        }

        [UnityTest]
        public IEnumerator Long_Field_IsSupported()
        {
            CreateWithComponent<SupportedLongBehaviour>();
            LogAssert.NoUnexpectedReceived();
            yield return null;
        }

        [UnityTest]
        public IEnumerator Bool_Field_IsSupported()
        {
            CreateWithComponent<SupportedBoolBehaviour>();
            LogAssert.NoUnexpectedReceived();
            yield return null;
        }

        // ── Supported property types ─────────────────────────────────────

        [UnityTest]
        public IEnumerator Float_Property_IsSupported()
        {
            CreateWithComponent<SupportedFloatPropertyBehaviour>();
            LogAssert.NoUnexpectedReceived();
            yield return null;
        }

        [UnityTest]
        public IEnumerator Int_Property_IsSupported()
        {
            CreateWithComponent<SupportedIntPropertyBehaviour>();
            LogAssert.NoUnexpectedReceived();
            yield return null;
        }

        // ── Unsupported types ────────────────────────────────────────────

        [UnityTest]
        public IEnumerator String_Field_IsRejected()
        {
            LogAssert.Expect(LogType.Error, new System.Text.RegularExpressions.Regex("Cannot cast.*val.*float.*ignored"));
            CreateWithComponent<UnsupportedStringBehaviour>();
            yield return null;
        }

        [UnityTest]
        public IEnumerator String_Property_IsRejected()
        {
            LogAssert.Expect(LogType.Error, new System.Text.RegularExpressions.Regex("Cannot cast.*Val.*float.*ignored"));
            CreateWithComponent<UnsupportedStringPropertyBehaviour>();
            yield return null;
        }
    }
}
