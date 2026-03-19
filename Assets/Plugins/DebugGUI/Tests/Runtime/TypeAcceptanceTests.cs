using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace WeavUtils.Tests
{
    // ── Stub MonoBehaviours (one per type × member kind) ─────────────
    // Fields
    public class FloatFieldBehaviour    : MonoBehaviour { [DebugGUIGraph] public float      val = 1f; }
    public class IntFieldBehaviour      : MonoBehaviour { [DebugGUIGraph] public int        val = 1; }
    public class DoubleFieldBehaviour   : MonoBehaviour { [DebugGUIGraph] public double     val = 1.0; }
    public class ByteFieldBehaviour     : MonoBehaviour { [DebugGUIGraph] public byte       val = 1; }
    public class ShortFieldBehaviour    : MonoBehaviour { [DebugGUIGraph] public short      val = 1; }
    public class LongFieldBehaviour     : MonoBehaviour { [DebugGUIGraph] public long       val = 1; }
    public class BoolFieldBehaviour     : MonoBehaviour { [DebugGUIGraph] public bool       val = true; }
    public class Vector2FieldBehaviour  : MonoBehaviour { [DebugGUIGraph] public Vector2    val = Vector2.one; }
    public class Vector2IntFieldBehaviour : MonoBehaviour { [DebugGUIGraph] public Vector2Int val = Vector2Int.one; }
    public class Vector3FieldBehaviour  : MonoBehaviour { [DebugGUIGraph] public Vector3    val = Vector3.one; }
    public class Vector3IntFieldBehaviour : MonoBehaviour { [DebugGUIGraph] public Vector3Int val = Vector3Int.one; }
    public class Vector4FieldBehaviour  : MonoBehaviour { [DebugGUIGraph] public Vector4    val = Vector4.one; }
    public class QuaternionFieldBehaviour : MonoBehaviour { [DebugGUIGraph] public Quaternion val = Quaternion.identity; }
    public class ColorFieldBehaviour    : MonoBehaviour { [DebugGUIGraph] public Color      val = Color.white; }
    public class Color32FieldBehaviour  : MonoBehaviour { [DebugGUIGraph] public Color32    val = new Color32(255, 255, 255, 255); }
    public class StringFieldBehaviour   : MonoBehaviour { [DebugGUIGraph] public string     val = "hello"; }

    // Properties
    public class FloatPropertyBehaviour    : MonoBehaviour { [DebugGUIGraph] public float      Val => 1f; }
    public class IntPropertyBehaviour      : MonoBehaviour { [DebugGUIGraph] public int        Val => 1; }
    public class DoublePropertyBehaviour   : MonoBehaviour { [DebugGUIGraph] public double     Val => 1.0; }
    public class BytePropertyBehaviour     : MonoBehaviour { [DebugGUIGraph] public byte       Val => 1; }
    public class ShortPropertyBehaviour    : MonoBehaviour { [DebugGUIGraph] public short      Val => 1; }
    public class LongPropertyBehaviour     : MonoBehaviour { [DebugGUIGraph] public long       Val => 1; }
    public class BoolPropertyBehaviour     : MonoBehaviour { [DebugGUIGraph] public bool       Val => true; }
    public class Vector2PropertyBehaviour  : MonoBehaviour { [DebugGUIGraph] public Vector2    Val => Vector2.one; }
    public class Vector2IntPropertyBehaviour : MonoBehaviour { [DebugGUIGraph] public Vector2Int Val => Vector2Int.one; }
    public class Vector3PropertyBehaviour  : MonoBehaviour { [DebugGUIGraph] public Vector3    Val => Vector3.one; }
    public class Vector3IntPropertyBehaviour : MonoBehaviour { [DebugGUIGraph] public Vector3Int Val => Vector3Int.one; }
    public class Vector4PropertyBehaviour  : MonoBehaviour { [DebugGUIGraph] public Vector4    Val => Vector4.one; }
    public class QuaternionPropertyBehaviour : MonoBehaviour { [DebugGUIGraph] public Quaternion Val => Quaternion.identity; }
    public class ColorPropertyBehaviour    : MonoBehaviour { [DebugGUIGraph] public Color      Val => Color.white; }
    public class Color32PropertyBehaviour  : MonoBehaviour { [DebugGUIGraph] public Color32    Val => new Color32(255, 255, 255, 255); }
    public class StringPropertyBehaviour   : MonoBehaviour { [DebugGUIGraph] public string     Val => "hello"; }

    // Methods
    public class FloatMethodBehaviour    : MonoBehaviour { [DebugGUIGraph] public float      GetVal() => 1f; }
    public class IntMethodBehaviour      : MonoBehaviour { [DebugGUIGraph] public int        GetVal() => 1; }
    public class DoubleMethodBehaviour   : MonoBehaviour { [DebugGUIGraph] public double     GetVal() => 1.0; }
    public class ByteMethodBehaviour     : MonoBehaviour { [DebugGUIGraph] public byte       GetVal() => 1; }
    public class ShortMethodBehaviour    : MonoBehaviour { [DebugGUIGraph] public short      GetVal() => 1; }
    public class LongMethodBehaviour     : MonoBehaviour { [DebugGUIGraph] public long       GetVal() => 1; }
    public class BoolMethodBehaviour     : MonoBehaviour { [DebugGUIGraph] public bool       GetVal() => true; }
    public class Vector2MethodBehaviour  : MonoBehaviour { [DebugGUIGraph] public Vector2    GetVal() => Vector2.one; }
    public class Vector2IntMethodBehaviour : MonoBehaviour { [DebugGUIGraph] public Vector2Int GetVal() => Vector2Int.one; }
    public class Vector3MethodBehaviour  : MonoBehaviour { [DebugGUIGraph] public Vector3    GetVal() => Vector3.one; }
    public class Vector3IntMethodBehaviour : MonoBehaviour { [DebugGUIGraph] public Vector3Int GetVal() => Vector3Int.one; }
    public class Vector4MethodBehaviour  : MonoBehaviour { [DebugGUIGraph] public Vector4    GetVal() => Vector4.one; }
    public class QuaternionMethodBehaviour : MonoBehaviour { [DebugGUIGraph] public Quaternion GetVal() => Quaternion.identity; }
    public class ColorMethodBehaviour    : MonoBehaviour { [DebugGUIGraph] public Color      GetVal() => Color.white; }
    public class Color32MethodBehaviour  : MonoBehaviour { [DebugGUIGraph] public Color32    GetVal() => new Color32(255, 255, 255, 255); }
    public class StringMethodBehaviour   : MonoBehaviour { [DebugGUIGraph] public string     GetVal() => "hello"; }

    public class TypeAcceptanceTests
    {
        GameObject testObject;

        // ── Test case tables ─────────────────────────────────────────

        static IEnumerable<TestCaseData> SupportedFieldCases()
        {
            yield return new TestCaseData(typeof(FloatFieldBehaviour)).SetName("float");
            yield return new TestCaseData(typeof(IntFieldBehaviour)).SetName("int");
            yield return new TestCaseData(typeof(DoubleFieldBehaviour)).SetName("double");
            yield return new TestCaseData(typeof(ByteFieldBehaviour)).SetName("byte");
            yield return new TestCaseData(typeof(ShortFieldBehaviour)).SetName("short");
            yield return new TestCaseData(typeof(LongFieldBehaviour)).SetName("long");
            yield return new TestCaseData(typeof(BoolFieldBehaviour)).SetName("bool");
            yield return new TestCaseData(typeof(Vector2FieldBehaviour)).SetName("Vector2");
            yield return new TestCaseData(typeof(Vector2IntFieldBehaviour)).SetName("Vector2Int");
            yield return new TestCaseData(typeof(Vector3FieldBehaviour)).SetName("Vector3");
            yield return new TestCaseData(typeof(Vector3IntFieldBehaviour)).SetName("Vector3Int");
            yield return new TestCaseData(typeof(Vector4FieldBehaviour)).SetName("Vector4");
            yield return new TestCaseData(typeof(QuaternionFieldBehaviour)).SetName("Quaternion");
            yield return new TestCaseData(typeof(ColorFieldBehaviour)).SetName("Color");
            yield return new TestCaseData(typeof(Color32FieldBehaviour)).SetName("Color32");
        }

        static IEnumerable<TestCaseData> SupportedPropertyCases()
        {
            yield return new TestCaseData(typeof(FloatPropertyBehaviour)).SetName("float");
            yield return new TestCaseData(typeof(IntPropertyBehaviour)).SetName("int");
            yield return new TestCaseData(typeof(DoublePropertyBehaviour)).SetName("double");
            yield return new TestCaseData(typeof(BytePropertyBehaviour)).SetName("byte");
            yield return new TestCaseData(typeof(ShortPropertyBehaviour)).SetName("short");
            yield return new TestCaseData(typeof(LongPropertyBehaviour)).SetName("long");
            yield return new TestCaseData(typeof(BoolPropertyBehaviour)).SetName("bool");
            yield return new TestCaseData(typeof(Vector2PropertyBehaviour)).SetName("Vector2");
            yield return new TestCaseData(typeof(Vector2IntPropertyBehaviour)).SetName("Vector2Int");
            yield return new TestCaseData(typeof(Vector3PropertyBehaviour)).SetName("Vector3");
            yield return new TestCaseData(typeof(Vector3IntPropertyBehaviour)).SetName("Vector3Int");
            yield return new TestCaseData(typeof(Vector4PropertyBehaviour)).SetName("Vector4");
            yield return new TestCaseData(typeof(QuaternionPropertyBehaviour)).SetName("Quaternion");
            yield return new TestCaseData(typeof(ColorPropertyBehaviour)).SetName("Color");
            yield return new TestCaseData(typeof(Color32PropertyBehaviour)).SetName("Color32");
        }

        static IEnumerable<TestCaseData> SupportedMethodCases()
        {
            yield return new TestCaseData(typeof(FloatMethodBehaviour)).SetName("float");
            yield return new TestCaseData(typeof(IntMethodBehaviour)).SetName("int");
            yield return new TestCaseData(typeof(DoubleMethodBehaviour)).SetName("double");
            yield return new TestCaseData(typeof(ByteMethodBehaviour)).SetName("byte");
            yield return new TestCaseData(typeof(ShortMethodBehaviour)).SetName("short");
            yield return new TestCaseData(typeof(LongMethodBehaviour)).SetName("long");
            yield return new TestCaseData(typeof(BoolMethodBehaviour)).SetName("bool");
            yield return new TestCaseData(typeof(Vector2MethodBehaviour)).SetName("Vector2");
            yield return new TestCaseData(typeof(Vector2IntMethodBehaviour)).SetName("Vector2Int");
            yield return new TestCaseData(typeof(Vector3MethodBehaviour)).SetName("Vector3");
            yield return new TestCaseData(typeof(Vector3IntMethodBehaviour)).SetName("Vector3Int");
            yield return new TestCaseData(typeof(Vector4MethodBehaviour)).SetName("Vector4");
            yield return new TestCaseData(typeof(QuaternionMethodBehaviour)).SetName("Quaternion");
            yield return new TestCaseData(typeof(ColorMethodBehaviour)).SetName("Color");
            yield return new TestCaseData(typeof(Color32MethodBehaviour)).SetName("Color32");
        }

        static IEnumerable<TestCaseData> UnsupportedCases()
        {
            yield return new TestCaseData(typeof(StringFieldBehaviour)).SetName("string field");
            yield return new TestCaseData(typeof(StringPropertyBehaviour)).SetName("string property");
            yield return new TestCaseData(typeof(StringMethodBehaviour)).SetName("string method");
        }

        // ── Setup / teardown ─────────────────────────────────────────

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            yield return null;
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            if (testObject != null)
                UnityEngine.Object.Destroy(testObject);
            DebugGUI.ForceReinitializeAttributes();
            yield return null;
        }

        void CreateWithComponent(Type type)
        {
            testObject = new GameObject($"TypeTest_{type.Name}");
            testObject.AddComponent(type);
            DebugGUI.ForceReinitializeAttributes();
        }

        // ── Parameterized tests ──────────────────────────────────────

        [UnityTest, TestCaseSource(nameof(SupportedFieldCases))]
        public IEnumerator Field_IsSupported(Type behaviourType)
        {
            CreateWithComponent(behaviourType);
            LogAssert.NoUnexpectedReceived();
            yield return null;
        }

        [UnityTest, TestCaseSource(nameof(SupportedPropertyCases))]
        public IEnumerator Property_IsSupported(Type behaviourType)
        {
            CreateWithComponent(behaviourType);
            LogAssert.NoUnexpectedReceived();
            yield return null;
        }

        [UnityTest, TestCaseSource(nameof(SupportedMethodCases))]
        public IEnumerator Method_IsSupported(Type behaviourType)
        {
            CreateWithComponent(behaviourType);
            LogAssert.NoUnexpectedReceived();
            yield return null;
        }

        [UnityTest, TestCaseSource(nameof(UnsupportedCases))]
        public IEnumerator Unsupported_IsRejected(Type behaviourType)
        {
            LogAssert.Expect(LogType.Error, new System.Text.RegularExpressions.Regex("not supported"));
            CreateWithComponent(behaviourType);
            yield return null;
        }
    }
}
