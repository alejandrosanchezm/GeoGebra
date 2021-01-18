using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;

namespace GeoGebra
{
    /// <summary>
    /// Clase encargada de calcular el valor de los puntos
    /// a partir de una expresión matemática.
    /// </summary>
    public class Matematica
    {
        private const char GeqSign = (char)8805;
        private const char LeqSign = (char)8804;
        private const char NeqSign = (char)8800;

        /// <summary>
        /// Contiene todos los operadores binarios definidos para el analizador.
        /// </summary>
        public Dictionary<string, Func<double, double, double>> Operators { get; set; }

        /// <summary>
        /// Contiene todas las funciones definidas para el analizador.
        /// </summary>
        public Dictionary<string, Func<double[], double>> LocalFunctions { get; set; }

        /// <summary>
        /// Contiene todas las variables definidas para el analizador.
        /// </summary>
        public Dictionary<string, double> LocalVariables { get; set; }

        /// <summary>
        /// La información cultural que se utilizará al analizar expresiones.
        /// </summary>
        [Obsolete]
        public CultureInfo CultureInfo { get; set; }

        /// <summary>
        /// Un generador de números aleatorios que pueden utilizar funciones y operadores.
        /// </summary>
        public Random Random { get; set; } = new Random();

        /// <summary>
        /// La palabra clave que se utilizará para las declaraciones de variables al analizar. El valor predeterminado es "let".
        /// </summary>
        public string VariableDeclarator { get; set; } = "let";

        /// <summary>
        /// Constructor de la Clase. Para el funcionamiento base, no necesita argumentos.
        /// </summary>
        /// <param name="loadPreDefinedFunctions"></param>
        /// <param name="loadPreDefinedOperators"></param>
        /// <param name="loadPreDefinedVariables"></param>
        /// <param name="cultureInfo"></param>
        public Matematica(bool loadPreDefinedFunctions = true, bool loadPreDefinedOperators = true,
                          bool loadPreDefinedVariables = true, CultureInfo cultureInfo = null)
        {
            if (loadPreDefinedOperators)
            {
                Operators = new Dictionary<string, Func<double, double, double>>
                {
                    ["^"] = Math.Pow,
                    ["%"] = (a, b) => a % b,
                    [":"] = (a, b) =>
                    {
                        if (b != 0)
                            return a / b;
                        else if (a > 0)
                            return double.PositiveInfinity;
                        else if (a < 0)
                            return double.NegativeInfinity;
                        else
                            return double.NaN;
                    },
                    ["/"] = (a, b) =>
                    {
                        if (b != 0)
                            return a / b;
                        else if (a > 0)
                            return double.PositiveInfinity;
                        else if (a < 0)
                            return double.NegativeInfinity;
                        else
                            return double.NaN;
                    },
                    ["*"] = (a, b) => a * b,
                    ["-"] = (a, b) => a - b,
                    ["+"] = (a, b) => a + b,

                    [">"] = (a, b) => a > b ? 1 : 0,
                    ["<"] = (a, b) => a < b ? 1 : 0,
                    ["" + GeqSign] = (a, b) => a > b || Math.Abs(a - b) < 0.00000001 ? 1 : 0,
                    ["" + LeqSign] = (a, b) => a < b || Math.Abs(a - b) < 0.00000001 ? 1 : 0,
                    ["" + NeqSign] = (a, b) => Math.Abs(a - b) < 0.00000001 ? 0 : 1,
                    ["="] = (a, b) => Math.Abs(a - b) < 0.00000001 ? 1 : 0
                };
            }
            else
            {
                Operators = new Dictionary<string, Func<double, double, double>>();
            }

            if (loadPreDefinedFunctions)
            {
                LocalFunctions = new Dictionary<string, Func<double[], double>>
                {
                    ["abs"] = inputs => Math.Abs(inputs[0]),

                    ["cos"] = inputs => Math.Cos(inputs[0]),
                    ["cosh"] = inputs => Math.Cosh(inputs[0]),
                    ["acos"] = inputs => Math.Acos(inputs[0]),
                    ["arccos"] = inputs => Math.Acos(inputs[0]),

                    ["sin"] = inputs => Math.Sin(inputs[0]),
                    ["sinh"] = inputs => Math.Sinh(inputs[0]),
                    ["asin"] = inputs => Math.Asin(inputs[0]),
                    ["arcsin"] = inputs => Math.Asin(inputs[0]),

                    ["tan"] = inputs => Math.Tan(inputs[0]),
                    ["tanh"] = inputs => Math.Tanh(inputs[0]),
                    ["atan"] = inputs => Math.Atan(inputs[0]),
                    ["arctan"] = inputs => Math.Atan(inputs[0]),

                    ["sqrt"] = inputs => Math.Sqrt(inputs[0]),
                    ["pow"] = inputs => Math.Pow(inputs[0], inputs[1]),
                    ["root"] = inputs => Math.Pow(inputs[0], 1 / inputs[1]),
                    ["rem"] = inputs => Math.IEEERemainder(inputs[0], inputs[1]),

                    ["sign"] = inputs => Math.Sign(inputs[0]),
                    ["exp"] = inputs => Math.Exp(inputs[0]),

                    ["floor"] = inputs => Math.Floor(inputs[0]),
                    ["ceil"] = inputs => Math.Ceiling(inputs[0]),
                    ["ceiling"] = inputs => Math.Ceiling(inputs[0]),
                    ["round"] = inputs => Math.Round(inputs[0], MidpointRounding.AwayFromZero),
                    ["truncate"] = inputs => inputs[0] < 0 ? -Math.Floor(-inputs[0]) : Math.Floor(inputs[0]),

                    ["log"] = inputs =>
                    {
                        switch (inputs.Length)
                        {
                            case 1:
                                return Math.Log10(inputs[0]);
                            case 2:
                                return Math.Log(inputs[0], inputs[1]);
                            default:
                                return 0;
                        }
                    },

                    ["random"] = inputs =>
                    {
                        if (inputs.Length == 0 || (inputs.Length == 1 && inputs[0] == 0))
                        {
                            inputs = new double[1];
                            inputs[0] = 1;
                        }
                        if (inputs.Length == 2)
                        {
                            return Random.NextDouble() * (inputs[1] - inputs[0]) + inputs[0];
                        }
                        return Random.NextDouble() * inputs[0];
                    },

                    ["ln"] = inputs => Math.Log(inputs[0])
                };
            }
            else
            {
                LocalFunctions = new Dictionary<string, Func<double[], double>>();
            }

            if (loadPreDefinedVariables)
            {
                LocalVariables = new Dictionary<string, double>
                {
                    ["pi"] = 3.14159265358979,
                    ["tao"] = 6.28318530717959,

                    ["e"] = 2.71828182845905,
                    ["phi"] = 1.61803398874989,
                    ["major"] = 0.61803398874989,
                    ["minor"] = 0.38196601125011,

                    ["pitograd"] = 57.2957795130823,
                    ["piofgrad"] = 0.01745329251994
                };
            }
            else
            {
                LocalVariables = new Dictionary<string, double>();
            }

            CultureInfo = cultureInfo ?? CultureInfo.InvariantCulture;
        }

        [Obsolete]
        public double Parse(string mathExpression)
        {
            return MathParserLogic(Lexer(mathExpression));
        }

        public double ProgrammaticallyParse(string mathExpression, bool correctExpression = true, bool identifyComments = true)
        {
            if (mathExpression.Contains("/0"))
            {
                throw new MathParserException("Cannot divide by 0.");
            }
            if (identifyComments)
            {
                // Delete Comments #{Comment}#
                mathExpression = System.Text.RegularExpressions.Regex.Replace(mathExpression, "#\\{.*?\\}#", "");

                // Delete Comments #Comment
                mathExpression = System.Text.RegularExpressions.Regex.Replace(mathExpression, "#.*$", "");
            }

            if (correctExpression)
            {
                // this refers to the Correction function which will correct stuff like artn to arctan, etc.
                mathExpression = Correction(mathExpression);
            }

            string varName;
            double varValue;

            if (mathExpression.Contains(VariableDeclarator))
            {

                if (mathExpression.Contains("be"))
                {
                    varName = mathExpression.Substring(mathExpression.IndexOf(VariableDeclarator, StringComparison.Ordinal) + 3,
                        mathExpression.IndexOf("be", StringComparison.Ordinal) -
                        mathExpression.IndexOf(VariableDeclarator, StringComparison.Ordinal) - 3);
                    mathExpression = mathExpression.Replace(varName + "be", "");
                }
                else
                {
                    varName = mathExpression.Substring(mathExpression.IndexOf(VariableDeclarator, StringComparison.Ordinal) + 3,
                        mathExpression.IndexOf("=", StringComparison.Ordinal) -
                        mathExpression.IndexOf(VariableDeclarator, StringComparison.Ordinal) - 3);
                    mathExpression = mathExpression.Replace(varName + "=", "");
                }

                varName = varName.Replace(" ", "");
                mathExpression = mathExpression.Replace(VariableDeclarator, "");

                varValue = Parse(mathExpression);

                if (LocalVariables.ContainsKey(varName))
                {
                    LocalVariables[varName] = varValue;
                }
                else
                {
                    LocalVariables.Add(varName, varValue);
                }

                return varValue;
            }

            if (!mathExpression.Contains(":="))
            {
                return Parse(mathExpression);
            }

            //mathExpression = mathExpression.Replace(" ", ""); // remove white space
            varName = mathExpression.Substring(0, mathExpression.IndexOf(":=", StringComparison.Ordinal));
            mathExpression = mathExpression.Replace(varName + ":=", "");

            varValue = Parse(mathExpression);
            varName = varName.Replace(" ", "");

            if (LocalVariables.ContainsKey(varName))
            {
                LocalVariables[varName] = varValue;
            }
            else
            {
                LocalVariables.Add(varName, varValue);
            }

            return varValue;
        }
        
        public IReadOnlyCollection<string> GetTokens(string mathExpression)
        {
            return Lexer(mathExpression);
        }

        private string Correction(string input)
        {
            // Word corrections

            input = System.Text.RegularExpressions.Regex.Replace(input, "\\b(sqr|sqrt)\\b", "sqrt", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            input = System.Text.RegularExpressions.Regex.Replace(input, "\\b(atan2|arctan2)\\b", "arctan2", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            return input;
        }

        private List<string> Lexer(string expr)
        {
            var token = "";
            var tokens = new List<string>();

            expr = expr.Replace("+-", "-");
            expr = expr.Replace("-+", "-");
            expr = expr.Replace("--", "+");
            expr = expr.Replace("==", "=");
            expr = expr.Replace(">=", "" + GeqSign);
            expr = expr.Replace("<=", "" + LeqSign);
            expr = expr.Replace("!=", "" + NeqSign);

            for (var i = 0; i < expr.Length; i++)
            {
                var ch = expr[i];

                if (char.IsWhiteSpace(ch))
                {
                    continue;
                }

                if (char.IsLetter(ch))
                {
                    if (i != 0 && (char.IsDigit(expr[i - 1]) || expr[i - 1] == ')'))
                    {
                        tokens.Add("*");
                    }

                    token += ch;

                    while (i + 1 < expr.Length && char.IsLetterOrDigit(expr[i + 1]))
                    {
                        token += expr[++i];
                    }

                    tokens.Add(token);
                    token = "";

                    continue;
                }

                if (char.IsDigit(ch))
                {
                    token += ch;

                    while (i + 1 < expr.Length && (char.IsDigit(expr[i + 1]) || expr[i + 1] == '.'))
                    {
                        token += expr[++i];
                    }

                    tokens.Add(token);
                    token = "";

                    continue;
                }

                if (ch == '.')
                {
                    token += ch;

                    while (i + 1 < expr.Length && char.IsDigit(expr[i + 1]))
                    {
                        token += expr[++i];
                    }

                    tokens.Add(token);
                    token = "";

                    continue;
                }

                if (i + 1 < expr.Length &&
                    (ch == '-' || ch == '+') &&
                    char.IsDigit(expr[i + 1]) &&
                    (i == 0 || (tokens.Count > 0 && Operators.ContainsKey(tokens.Last())) || i - 1 > 0 && expr[i - 1] == '('))
                {
                    // if the above is true, then the token for that negative number will be "-1", not "-","1".
                    // to sum up, the above will be true if the minus sign is in front of the number, but
                    // at the beginning, for example, -1+2, or, when it is inside the brakets (-1), or when it comes after another operator.
                    // NOTE: this works for + as well!

                    token += ch;

                    while (i + 1 < expr.Length && (char.IsDigit(expr[i + 1]) || expr[i + 1] == '.'))
                    {
                        token += expr[++i];
                    }

                    tokens.Add(token);
                    token = "";

                    continue;
                }

                if (ch == '(')
                {
                    if (i != 0 && (char.IsDigit(expr[i - 1]) || char.IsDigit(expr[i - 1]) || expr[i - 1] == ')'))
                    {
                        tokens.Add("*");
                        tokens.Add("(");
                    }
                    else
                    {
                        tokens.Add("(");
                    }
                }
                else
                {
                    tokens.Add(ch.ToString());
                }
            }

            return tokens;
        }

        [Obsolete]
        private double MathParserLogic(List<string> tokens)
        {
            // Variables replacement
            for (var i = 0; i < tokens.Count; i++)
            {
                if (LocalVariables.Keys.Contains(tokens[i]))
                {
                    tokens[i] = LocalVariables[tokens[i]].ToString(CultureInfo);
                }
            }

            while (tokens.IndexOf("(") != -1)
            {
                // getting data between "(" and ")"
                var open = tokens.LastIndexOf("(");
                var close = tokens.IndexOf(")", open); // in case open is -1, i.e. no "(" // , open == 0 ? 0 : open - 1

                if (open >= close)
                {
                    throw new ArithmeticException("No closing bracket/parenthesis. Token: " + open.ToString(CultureInfo));
                }

                var roughExpr = new List<string>();

                for (var i = open + 1; i < close; i++)
                {
                    roughExpr.Add(tokens[i]);
                }

                double tmpResult;

                var args = new List<double>();
                var functionName = tokens[open == 0 ? 0 : open - 1];

                if (LocalFunctions.Keys.Contains(functionName))
                {
                    if (roughExpr.Contains(","))
                    {
                        // converting all arguments into a double array
                        for (var i = 0; i < roughExpr.Count; i++)
                        {
                            var defaultExpr = new List<string>();
                            var firstCommaOrEndOfExpression =
                                roughExpr.IndexOf(",", i) != -1
                                    ? roughExpr.IndexOf(",", i)
                                    : roughExpr.Count;

                            while (i < firstCommaOrEndOfExpression)
                            {
                                defaultExpr.Add(roughExpr[i++]);
                            }

                            args.Add(defaultExpr.Count == 0 ? 0 : BasicArithmeticalExpression(defaultExpr));
                        }

                        // finally, passing the arguments to the given function
                        tmpResult = double.Parse(LocalFunctions[functionName](args.ToArray()).ToString(CultureInfo), CultureInfo);
                    }
                    else
                    {
                        if (roughExpr.Count == 0)
                            tmpResult = LocalFunctions[functionName](new double[0]);
                        else
                        {
                            tmpResult = double.Parse(LocalFunctions[functionName](new[]
                            {
                                BasicArithmeticalExpression(roughExpr)
                            }).ToString(CultureInfo), CultureInfo);
                        }
                    }
                }
                else
                {
                    // if no function is need to execute following expression, pass it
                    // to the "BasicArithmeticalExpression" method.
                    tmpResult = BasicArithmeticalExpression(roughExpr);
                }

                // when all the calculations have been done
                // we replace the "opening bracket with the result"
                // and removing the rest.
                tokens[open] = tmpResult.ToString(CultureInfo);
                tokens.RemoveRange(open + 1, close - open);

                if (LocalFunctions.Keys.Contains(functionName))
                {
                    // if we also executed a function, removing
                    // the function name as well.
                    tokens.RemoveAt(open - 1);
                }
            }

            // at this point, we should have replaced all brackets
            // with the appropriate values, so we can simply
            // calculate the expression. it's not so complex
            // any more!
            return BasicArithmeticalExpression(tokens);
        }

        private double BasicArithmeticalExpression(List<string> tokens)
        {
            // PERFORMING A BASIC ARITHMETICAL EXPRESSION CALCULATION
            // THIS METHOD CAN ONLY OPERATE WITH NUMBERS AND OPERATORS
            // AND WILL NOT UNDERSTAND ANYTHING BEYOND THAT.

            double token0;
            double token1;

            switch (tokens.Count)
            {
                case 1:
                    if (!double.TryParse(tokens[0], NumberStyles.Number, CultureInfo, out token0))
                    {
                        throw new MathParserException("local variable " + tokens[0] + " is undefined");
                    }

                    return token0;
                case 2:
                    var op = tokens[0];

                    if (op == "-" || op == "+")
                    {
                        var first = op == "+" ? "" : (tokens[1].Substring(0, 1) == "-" ? "" : "-");

                        if (!double.TryParse(first + tokens[1], NumberStyles.Number, CultureInfo, out token1))
                        {
                            throw new MathParserException("local variable " + first + tokens[1] + " is undefined");
                        }

                        return token1;
                    }

                    if (!Operators.ContainsKey(op))
                    {
                        throw new MathParserException("operator " + op + " is not defined");
                    }

                    if (!double.TryParse(tokens[1], NumberStyles.Number, CultureInfo, out token1))
                    {
                        throw new MathParserException("local variable " + tokens[1] + " is undefined");
                    }

                    return Operators[op](0, token1);
                case 0:
                    return 0;
            }

            foreach (var op in Operators)
            {
                int opPlace;

                while ((opPlace = tokens.IndexOf(op.Key)) != -1)
                {
                    double rhs;

                    if (!double.TryParse(tokens[opPlace + 1], NumberStyles.Number, CultureInfo, out rhs))
                    {
                        throw new MathParserException("local variable " + tokens[opPlace + 1] + " is undefined");
                    }

                    if (op.Key == "-" && opPlace == 0)
                    {
                        var result = op.Value(0.0, rhs);
                        tokens[0] = result.ToString(CultureInfo);
                        tokens.RemoveRange(opPlace + 1, 1);
                    }
                    else
                    {
                        double lhs;

                        if (!double.TryParse(tokens[opPlace - 1], NumberStyles.Number, CultureInfo, out lhs))
                        {
                            throw new MathParserException("local variable " + tokens[opPlace - 1] + " is undefined");
                        }

                        var result = op.Value(lhs, rhs);
                        tokens[opPlace - 1] = result.ToString(CultureInfo);
                        tokens.RemoveRange(opPlace, 2);
                    }
                }
            }

            if (!double.TryParse(tokens[0], NumberStyles.Number, CultureInfo, out token0))
            {
                throw new MathParserException("local variable " + tokens[0] + " is undefined");
            }

            return token0;
        }

        /// <summary>
        /// Indica si es o no un número.
        /// </summary>
        /// <param name="c">Caracter a analizar.</param>
        /// <returns>
        /// Devuelve True si es número; False en caso contrario.
        /// </returns>
        public bool IsNumeric(char c)
        {
            int value;
            try
            {
                bool result = Int32.TryParse(c.ToString(), out value);
                return result;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        /// <summary>
        /// Indica si es o no un paréntesis o corchete.
        /// </summary>
        /// <param name="c">Caracter a analizar.</param>
        /// <returns>
        /// Devuelve True si es número; False en caso contrario.
        /// </returns>
        public bool IsSeparator(char c)
        {
            if (c == '(' || c == ')' || c == '[' || c == ']') return true;
            return false;
        }

        /// <summary>
        /// Devuelve la cadena en Lower Case, y sitúa el símbolo de multiplicación entre número, variable y paréntesis cuando es necesario.
        /// </summary>
        /// <param name="equation">Expresión a Preprocesar.</param>
        /// <param name="variable">Variable de la Expresión matemática.</param>
        /// <returns>
        /// Devuelve la cadena preprocesada.
        /// </returns>
        public string preprocessString(string equation, string variable)
        {
            if (equation.Length > 1)
            {
                equation = equation.ToLower();
                string tmp = equation;
                char currChar, prevChar;

                for (int i = 1; i < equation.Length; i++)
                {
                    currChar = equation[i];
                    prevChar = equation[i - 1];
                    if ((IsNumeric(currChar) && prevChar.ToString() == variable) || (IsNumeric(prevChar) && currChar.ToString() == variable))
                    {
                        string t1 = prevChar.ToString() + currChar.ToString();
                        string t2 = prevChar.ToString() + "*" + currChar.ToString();
                        tmp = tmp.Replace(t1, t2);
                    }
                }
                return tmp;
            }
            else return equation;
        }

    }

    public sealed class MathParserException : Exception
    {

        public MathParserException()
        {
        }

        public MathParserException(string message) : base(message)
        {
        }

        public MathParserException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

}