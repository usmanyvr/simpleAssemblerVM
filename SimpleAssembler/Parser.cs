using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleAssembler
{
    // All the supported instructions
    enum Command
    {
        NONE,
        ADD,
        MUL,
        MOV,
        PUSH,
        POP,
        CMP,
        JZ,
        JNZ,
        JPOS,
        OUTPUT,
        EXIT
    }

    // All the supported Command types
    enum CommandType
    {
        None,
        Data,
        Comparison,
        Jump,
        Exit
    }

    // Type of operand
    enum OperandType
    {
        None,
        MemoryLocation,
        Integer,
        ComparisonResult
    };

    // InstructionOperand: this would be used to undertsand each operand
    struct InstructionOperand
    {
        // In case of MemoryLocation, value would identify the memory location
        // For example, type = MemoryLocation, value = 4 will mean m4 memory location
        // type = Integer, value will represent integer value
        // type = ComparisonResult will not factor in value
        public OperandType type;
        public int value;

        public override string ToString()
        {
            return type + " : " + value;
        }
    }

    class InstructionStructure
    {
        Command cmd;
        InstructionOperand operand1;
        InstructionOperand operand2;
        int numOperands;

        public InstructionStructure()
        {
            cmd = Command.NONE;
            numOperands = 0;
        }

        public Command Operation
        {
            get
            {
                return cmd;
            }
            set
            {
                this.cmd = value;
            }
        }

        public InstructionOperand Operand1
        {
            get
            {
                return operand1;
            }
            set
            {
                this.operand1 = value;
                numOperands = numOperands + 1;
                numOperands = numOperands > 2 ? 2 : numOperands;
            }
        }

        public InstructionOperand Operand2
        {
            get
            {
                return operand2;
            }
            set
            {
                this.operand2 = value;
                numOperands = numOperands + 1;
                numOperands = numOperands > 2 ? 2 : numOperands;                
            }
        }

        public int NumberOfOperands
        {
            get
            {
                return numOperands;
            }
        }
    }

    // Parses the command
    class Parser
    {
        Dictionary<string, Command> stringToInstructionMap;

        public Parser()
        {
            stringToInstructionMap = new Dictionary<string, Command>
            {
                { "ADD", Command.ADD },
                { "MUL", Command.MUL },
                { "MOV", Command.MOV },
                { "PUSH", Command.PUSH },
                { "POP", Command.POP },
                { "CMP", Command.CMP },
                { "JZ", Command.JZ },
                { "JNZ", Command.JNZ },
                { "JPOS", Command.JPOS },
                { "OUTPUT", Command.OUTPUT },
                { "EXIT", Command.EXIT }
            };
        }

        string[] splitWithSemiColon(string inputString, int maxSplits = 2)
        {
            string[] tokens;
            if (string.IsNullOrWhiteSpace(inputString))
            {
                return null;
            }

            tokens = maxSplits == 0 ? inputString.Split(';') : inputString.Split(';', maxSplits);

            return tokens;
        }

        string[] splitWithComma(string inputString, int maxSplits = 0)
        {
            string[] tokens;
            if (string.IsNullOrWhiteSpace(inputString))
            {
                return null;
            }

            tokens = maxSplits == 0 ? inputString.Split(',') : inputString.Split(',', maxSplits);

            return tokens;
        }

        string[] splitWithSpace(string inputString, int maxSplits = 0)
        {
            string[] tokens;
            if (string.IsNullOrWhiteSpace(inputString))
            {
                return null;
            }

            tokens = maxSplits == 0 ? inputString.Split(' ') : inputString.Split(' ', maxSplits);

            return tokens;
        }

        InstructionOperand extractOperand(string argument)
        {
            InstructionOperand operand;
            operand.type = OperandType.None;
            operand.value = 0;
            argument = argument.Trim();

            try
            {
                // MemoryLocation type
                if (argument.StartsWith("M") || argument.StartsWith("m"))
                {
                    // m1 .... m8 are valid memory locations
                    if (argument.Length == 2)
                    {
                        operand.type = OperandType.MemoryLocation;
                        operand.value = Convert.ToInt32(argument[1]) - 48;
                    }
                    else
                    {
                        throw ExceptionFactory.getException(ExceptionMessage.InvalidMemoryLocation);
                    }
                }
                else
                {
                    operand.type = OperandType.Integer;
                    operand.value = Convert.ToInt32(argument);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return operand;
        }

        // returns true if instruction is processed fine, false otherwise.
        public InstructionStructure parseStatement(string statement)
        {            
            InstructionStructure instructionStructure = new InstructionStructure();
            InstructionOperand operand1, operand2;

            try
            {
                if (string.IsNullOrWhiteSpace(statement))
                {                    
                    instructionStructure.Operation = Command.NONE;
                    return instructionStructure;
                }

                string[] semiColonSeparatedString = splitWithSemiColon(statement);
                if(string.IsNullOrWhiteSpace(semiColonSeparatedString[0]))
                {
                    instructionStructure.Operation = Command.NONE;
                    return instructionStructure;                    
                }
                string[] tokens = splitWithSpace(semiColonSeparatedString[0], 2);
                string instruction = tokens[0].Trim().ToUpper();

                if (tokens == null)
                {
                    instructionStructure.Operation = Command.NONE;
                    return instructionStructure;
                }

                if (!stringToInstructionMap.ContainsKey(instruction))
                {
                    throw ExceptionFactory.getException(ExceptionMessage.InvalidCommand);
                }

                instructionStructure.Operation = stringToInstructionMap[instruction];

                // it means that arguments are there
                if (tokens.Length > 1)
                {
                    string[] arguments = splitWithComma(tokens[1]);

                    if (arguments.Length > 2)
                    {
                        throw ExceptionFactory.getException(ExceptionMessage.InvalidNumberOfParams);
                    }

                    operand1 = extractOperand(arguments[0]);
                    instructionStructure.Operand1 = operand1;

                    if (arguments.Length == 2)
                    {
                        operand2 = extractOperand(arguments[1]);
                        instructionStructure.Operand2 = operand2;
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            return instructionStructure;
        }
    }
}
