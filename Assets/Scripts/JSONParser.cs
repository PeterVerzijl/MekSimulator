using System;
using System.Collections.Generic;

public class JSONParser
{
	public static JSON Parse(string input) {
        JSON result;

        input.Trim();
        char[] chars = input.ToCharArray();

        int ptr = 0;
        result = ParseObject(chars, ref ptr);
        
        return result;
    }

    private static Value ParseValue(char[] chars, ref int ptr) {
        Value result = new Value();
        char c = chars[ptr];
        if (c == '{') {
            // Start object
            result.type = Value.Type.Object;
            result.value = ParseObject(chars, ref ptr);
        } else if (c == '[') {
            // Start array
            result.type = Value.Type.Array;
            result.value = ParseArray(chars, ref ptr);
        } else if (char.IsNumber(c) || c == '-') {
            // Number
            Number number = ParseNumber(chars, ref ptr);
            result.type = (number.isInteger)? Value.Type.Integer : Value.Type.Float;
            if (number.isInteger) {
                result.value = (int)number.integer;
            } else {
                result.value = (float)number.floatingPoint;
            }
        } else if (c == 't' || c == 'f') {
            // Boolean
            result.type = Value.Type.Boolean;
            result.value = ParseBool(chars, ref ptr);
        } else if (c == '"') {
            // Key or string
            result.type = Value.Type.String;
            result.value = ParseString(chars, ref ptr);
        } else {
            // error.
        }
        return result;
    }

    private static JSON ParseObject(char[] chars, ref int ptr) {
        JSON result = new JSON();
        char c = chars[ptr];
        while (true) {
            EatWhitespace(chars, ref ptr);
            Assert(chars[ptr] == '{');
            ++ptr;
            EatWhitespace(chars, ref ptr);
            c = chars[ptr];
            while (c != '}') {
                string key = ParseString(chars, ref ptr);
                c = chars[ptr];
                EatWhitespace(chars, ref ptr);
                c = chars[ptr];
                Assert(c == ':');
                ++ptr;
                EatWhitespace(chars, ref ptr);
                Value value = ParseValue(chars, ref ptr);
                EatWhitespace(chars, ref ptr);
                c = chars[ptr];
                // Add key value pair
                result.Add(key, value);
                EatWhitespace(chars, ref ptr);
                c = chars[ptr];
                if (c == ',') {
                    // Next
                    c = chars[++ptr];
                    EatWhitespace(chars, ref ptr);
                }
            }
            break;
        }
        ++ptr;
        return result;
    }

    private static void Assert(bool condition) {
        if (!condition) {
            throw new Exception();
        }
    } 

    private static void EatWhitespace(char[] chars, ref int ptr) {
        char c = chars[ptr];
        while (char.IsWhiteSpace(c)) {
            c = chars[++ptr];
        }
    }

    private static Value[] ParseArray(char[] chars, ref int ptr) {
        List<Value> result = new List<Value>();
        char c = chars[ptr];
        Assert(c == '[');
        c = chars[++ptr];
        EatWhitespace(chars, ref ptr);
        while (c != ']') {
            Value v = ParseValue(chars, ref ptr);
            result.Add(v);
            EatWhitespace(chars, ref ptr);
            c = chars[ptr];
            if (c == ',') {
                // Next
                c = chars[++ptr];
                EatWhitespace(chars, ref ptr);
            }
        }
        ++ptr;  // Skip the ] charcter
        return result.ToArray();
    }

    private static Number ParseNumber(char[] chars, ref int ptr) {
        Number result = new Number();
        result.isInteger = true;
        char c = chars[ptr];
        string buffer = "";
        while (char.IsDigit(c) || c == '-' || c == '.' || c == 'f') {
            if (c == '.' || c == 'f') {
                result.isInteger = false;
            }
            buffer += c;
            c = chars[++ptr];
        }
        if (result.isInteger) {
            int.TryParse(buffer, out result.integer);
        } else {
            float.TryParse(buffer, out result.floatingPoint);
        }
        return result;
    }

    static bool ParseBool(char[] chars, ref int ptr) {
        string buffer = "";
        char c = chars[ptr];
        while (char.IsLetter(c)) {
            buffer += c;
            c = chars[++ptr];
        }
        bool result;
        if (bool.TryParse(buffer, out result) == false) {
            // This is a big error!
        }
        return result;
    }

    static string ParseString(char[] chars, ref int ptr) {
        string result = "";
        char c = chars[ptr];
        Assert(c == '"');
        c = chars[++ptr];
        while (c != '"') {
            result += c;
            c = chars[++ptr];
        }
        ++ptr;
        return result;
    }

    private class Number {
        public bool isInteger = false;
        public int integer;
        public float floatingPoint;
    }
}

public class Value {
    public enum Type {
        Integer,
        Float,
        String,
        Boolean,
        Object,
        Array,
    };
    public Type type;
    public object value;
}

public class JSON { 
    public void Add(string key, Value value) {
        data.Add(key, value);
    }

    internal bool GetArray<T>(string target, out T[] result) {
        bool found = false;
        result = null;
        foreach(KeyValuePair<string, Value> entry in data) {
            if (entry.Key == target) {
                Value[] values = (Value[])entry.Value.value;
                T[] resultArray = new T[values.Length];
                for(int valueIndex = 0; valueIndex < values.Length; valueIndex++) {
                    resultArray[valueIndex] = (T)values[valueIndex].value;
                }
                result = resultArray;
            }
        }
        return found;
    }

    internal bool GetValue<T>(string target, out T result) {
        result = default(T);
        foreach(KeyValuePair<string, Value> entry in data) {
            if (entry.Key == target) {
                result = (T)entry.Value.value;
                return true;
            } else {
                if (entry.Value.type == Value.Type.Object) {
                    JSON obj = (JSON)entry.Value.value;
                    if (obj.GetValue(target, out result)) {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    public Dictionary<string, Value> data = new Dictionary<string, Value>();
}