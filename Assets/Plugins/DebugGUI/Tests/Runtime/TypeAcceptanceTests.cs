using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace WeavUtils.Tests
{
    // ── Field behaviours ─────────────────────────────────────────────
    public class FloatFieldBehaviour : MonoBehaviour
    {
        [DebugGUIGraph] public float val = 1f;
    }

    public class IntFieldBehaviour : MonoBehaviour
    {
        [DebugGUIGraph] public int val = 1;
    }

    public class DoubleFieldBehaviour : MonoBehaviour
    {
        [DebugGUIGraph] public double val = 1.0;
    }

    public class ByteFieldBehaviour : MonoBehaviour
    {
        [DebugGUIGraph] public byte val = 1;
    }

    public class ShortFieldBehaviour : MonoBehaviour
    {
        [DebugGUIGraph] public short val = 1;
    }

    public class LongFieldBehaviour : MonoBehaviour
    {
        [DebugGUIGraph] public long val = 1;
    }

    public class BoolFieldBehaviour : MonoBehaviour
    {
        [DebugGUIGraph] public bool val = true;
    }

    public class Vector2FieldBehaviour : MonoBehaviour
    {
        [DebugGUIGraph] public Vector2 val = Vector2.one;
    }

    public class Vector3FieldBehaviour : MonoBehaviour
    {
        [DebugGUIGraph] public Vector3 val = Vector3.one;
    }

    public class StringFieldBehaviour : MonoBehaviour
    {
        [DebugGUIGraph] public string val = "hello";
    }

    // ── Property behaviours ──────────────────────────────────────────
    public class FloatPropertyBehaviour : MonoBehaviour
    {
        [DebugGUIGraph] public float Val => 1f;
    }

    public class IntPropertyBehaviour : MonoBehaviour
    {
        [DebugGUIGraph] public int Val => 1;
    }

    public class DoublePropertyBehaviour : MonoBehaviour
    {
        [DebugGUIGraph] public double Val => 1.0;
    }

    public class BytePropertyBehaviour : MonoBehaviour
    {
        [DebugGUIGraph] public byte Val => 1;
    }

    public class ShortPropertyBehaviour : MonoBehaviour
    {
        [DebugGUIGraph] public short Val => 1;
    }

    public class LongPropertyBehaviour : MonoBehaviour
    {
        [DebugGUIGraph] public long Val => 1;
    }

    public class BoolPropertyBehaviour : MonoBehaviour
    {
        [DebugGUIGraph] public bool Val => true;
    }

    public class Vector2PropertyBehaviour : MonoBehaviour
    {
        [DebugGUIGraph] public Vector2 Val => Vector2.one;
    }

    public class Vector3PropertyBehaviour : MonoBehaviour
    {
        [DebugGUIGraph] public Vector3 Val => Vector3.one;
    }

    public class StringPropertyBehaviour : MonoBehaviour
    {
        [DebugGUIGraph] public string Val => "hello";
    }

    // ── Method behaviours ────────────────────────────────────────────
    public class FloatMethodBehaviour : MonoBehaviour
    {
        [DebugGUIGraph] public float GetVal() => 1f;
    }

    public class IntMethodBehaviour : MonoBehaviour
    {
        [DebugGUIGraph] public int GetVal() => 1;
    }

    public class DoubleMethodBehaviour : MonoBehaviour
    {
        [DebugGUIGraph] public double GetVal() => 1.0;
    }

    public class ByteMethodBehaviour : MonoBehaviour
    {
        [DebugGUIGraph] public byte GetVal() => 1;
    }

    public class ShortMethodBehaviour : MonoBehaviour
    {
        [DebugGUIGraph] public short GetVal() => 1;
    }

    public class LongMethodBehaviour : MonoBehaviour
    {
        [DebugGUIGraph] public long GetVal() => 1;
    }

    public class BoolMethodBehaviour : MonoBehaviour
    {
        [DebugGUIGraph] public bool GetVal() => true;
    }

    public class Vector2MethodBehaviour : MonoBehaviour
    {
        [DebugGUIGraph] public Vector2 GetVal() => Vector2.one;
    }

    public class Vector3MethodBehaviour : MonoBehaviour
    {
        [DebugGUIGraph] public Vector3 GetVal() => Vector3.one;
    }

    public class StringMethodBehaviour : MonoBehaviour
    {
        [DebugGUIGraph] public string GetVal() => "hello";
    }

    public class TypeAcceptanceTests
    {
        GameObject testObject;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
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

        // ── Supported field types ────────────────────────────────────

        [UnityTest] public IEnumerator Float_Field_IsSupported()
        { CreateWithComponent<FloatFieldBehaviour>(); LogAssert.NoUnexpectedReceived(); yield return null; }

        [UnityTest] public IEnumerator Int_Field_IsSupported()
        { CreateWithComponent<IntFieldBehaviour>(); LogAssert.NoUnexpectedReceived(); yield return null; }

        [UnityTest] public IEnumerator Double_Field_IsSupported()
        { CreateWithComponent<DoubleFieldBehaviour>(); LogAssert.NoUnexpectedReceived(); yield return null; }

        [UnityTest] public IEnumerator Byte_Field_IsSupported()
        { CreateWithComponent<ByteFieldBehaviour>(); LogAssert.NoUnexpectedReceived(); yield return null; }

        [UnityTest] public IEnumerator Short_Field_IsSupported()
        { CreateWithComponent<ShortFieldBehaviour>(); LogAssert.NoUnexpectedReceived(); yield return null; }

        [UnityTest] public IEnumerator Long_Field_IsSupported()
        { CreateWithComponent<LongFieldBehaviour>(); LogAssert.NoUnexpectedReceived(); yield return null; }

        [UnityTest] public IEnumerator Bool_Field_IsSupported()
        { CreateWithComponent<BoolFieldBehaviour>(); LogAssert.NoUnexpectedReceived(); yield return null; }

        [UnityTest] public IEnumerator Vector2_Field_IsSupported()
        { CreateWithComponent<Vector2FieldBehaviour>(); LogAssert.NoUnexpectedReceived(); yield return null; }

        [UnityTest] public IEnumerator Vector3_Field_IsSupported()
        { CreateWithComponent<Vector3FieldBehaviour>(); LogAssert.NoUnexpectedReceived(); yield return null; }

        // ── Supported property types ─────────────────────────────────

        [UnityTest] public IEnumerator Float_Property_IsSupported()
        { CreateWithComponent<FloatPropertyBehaviour>(); LogAssert.NoUnexpectedReceived(); yield return null; }

        [UnityTest] public IEnumerator Int_Property_IsSupported()
        { CreateWithComponent<IntPropertyBehaviour>(); LogAssert.NoUnexpectedReceived(); yield return null; }

        [UnityTest] public IEnumerator Double_Property_IsSupported()
        { CreateWithComponent<DoublePropertyBehaviour>(); LogAssert.NoUnexpectedReceived(); yield return null; }

        [UnityTest] public IEnumerator Byte_Property_IsSupported()
        { CreateWithComponent<BytePropertyBehaviour>(); LogAssert.NoUnexpectedReceived(); yield return null; }

        [UnityTest] public IEnumerator Short_Property_IsSupported()
        { CreateWithComponent<ShortPropertyBehaviour>(); LogAssert.NoUnexpectedReceived(); yield return null; }

        [UnityTest] public IEnumerator Long_Property_IsSupported()
        { CreateWithComponent<LongPropertyBehaviour>(); LogAssert.NoUnexpectedReceived(); yield return null; }

        [UnityTest] public IEnumerator Bool_Property_IsSupported()
        { CreateWithComponent<BoolPropertyBehaviour>(); LogAssert.NoUnexpectedReceived(); yield return null; }

        [UnityTest] public IEnumerator Vector2_Property_IsSupported()
        { CreateWithComponent<Vector2PropertyBehaviour>(); LogAssert.NoUnexpectedReceived(); yield return null; }

        [UnityTest] public IEnumerator Vector3_Property_IsSupported()
        { CreateWithComponent<Vector3PropertyBehaviour>(); LogAssert.NoUnexpectedReceived(); yield return null; }

        // ── Supported method types ───────────────────────────────────

        [UnityTest] public IEnumerator Float_Method_IsSupported()
        { CreateWithComponent<FloatMethodBehaviour>(); LogAssert.NoUnexpectedReceived(); yield return null; }

        [UnityTest] public IEnumerator Int_Method_IsSupported()
        { CreateWithComponent<IntMethodBehaviour>(); LogAssert.NoUnexpectedReceived(); yield return null; }

        [UnityTest] public IEnumerator Double_Method_IsSupported()
        { CreateWithComponent<DoubleMethodBehaviour>(); LogAssert.NoUnexpectedReceived(); yield return null; }

        [UnityTest] public IEnumerator Byte_Method_IsSupported()
        { CreateWithComponent<ByteMethodBehaviour>(); LogAssert.NoUnexpectedReceived(); yield return null; }

        [UnityTest] public IEnumerator Short_Method_IsSupported()
        { CreateWithComponent<ShortMethodBehaviour>(); LogAssert.NoUnexpectedReceived(); yield return null; }

        [UnityTest] public IEnumerator Long_Method_IsSupported()
        { CreateWithComponent<LongMethodBehaviour>(); LogAssert.NoUnexpectedReceived(); yield return null; }

        [UnityTest] public IEnumerator Bool_Method_IsSupported()
        { CreateWithComponent<BoolMethodBehaviour>(); LogAssert.NoUnexpectedReceived(); yield return null; }

        [UnityTest] public IEnumerator Vector2_Method_IsSupported()
        { CreateWithComponent<Vector2MethodBehaviour>(); LogAssert.NoUnexpectedReceived(); yield return null; }

        [UnityTest] public IEnumerator Vector3_Method_IsSupported()
        { CreateWithComponent<Vector3MethodBehaviour>(); LogAssert.NoUnexpectedReceived(); yield return null; }

        // ── Unsupported types ────────────────────────────────────────

        [UnityTest] public IEnumerator String_Field_IsRejected()
        {
            LogAssert.Expect(LogType.Error, new System.Text.RegularExpressions.Regex("not supported"));
            CreateWithComponent<StringFieldBehaviour>();
            yield return null;
        }

        [UnityTest] public IEnumerator String_Property_IsRejected()
        {
            LogAssert.Expect(LogType.Error, new System.Text.RegularExpressions.Regex("not supported"));
            CreateWithComponent<StringPropertyBehaviour>();
            yield return null;
        }

        [UnityTest] public IEnumerator String_Method_IsRejected()
        {
            LogAssert.Expect(LogType.Error, new System.Text.RegularExpressions.Regex("not supported"));
            CreateWithComponent<StringMethodBehaviour>();
            yield return null;
        }
    }
}
