using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleAssembler
{
    // store comparison result using an enum to be used in following jump instruction
    enum ComparisonResult
    {
        Equal,
        GreaterThan,
        LessThan
    }

    // MachineState represents state of machine's memory at any given time.
    // This includes memory (m1...m8) and stack
    class MachineState
    {
        // memory locations m1 ... m8
        int[] memory;

        // local stack
        Stack<int> stack;

        // store last instruction type
        public CommandType lastInstructionType;

        public ComparisonResult comparisonResult;

        public MachineState()
        {
            memory = new int[8];
            stack = new Stack<int>();
            lastInstructionType = CommandType.None;
        }

        public int readMemory(int address)
        {
            // valid addresses start from 1
            if( address <= 0 || address > memory.Length)
            {
                throw ExceptionFactory.getException(ExceptionMessage.InvalidMemoryLocation);
            }

            return memory[address - 1];
        }

        // 1 to 8 are valid memory addresses
        public int readMemory(InstructionOperand instructionOperand)
        {
            if(instructionOperand.type != OperandType.MemoryLocation)
            {
                throw ExceptionFactory.getException(ExceptionMessage.InvalidOperand);
            }

            return readMemory(instructionOperand.value);
        }

        public void writeMemory(int address, int value)
        {
            if (address <= 0 || address > memory.Length)
            {
                throw ExceptionFactory.getException(ExceptionMessage.InvalidMemoryLocation);
            }
            memory[address - 1] = value;
        }

        public void push(int value)
        {
            if(stack == null)
            {
                throw ExceptionFactory.getException(ExceptionMessage.StackNotReady);
            }

            stack.Push(value);
        }

        public int pop()
        {
            if(stack == null || stack.Count == 0)
            {
                throw ExceptionFactory.getException(ExceptionMessage.StackNotReady);
            }

            return stack.Pop();
        }

        public int StackSize
        {
            get
            {
                if(stack == null)
                {
                    throw ExceptionFactory.getException(ExceptionMessage.StackNotReady);
                }

                return stack.Count;
            }
        }
    }

    // VirtualMachine is to be instantiated by any driver program to run the commands
    // This contains all the functions as well as machine state at any given time.
    class VirtualMachine
    {
        MachineState state;
        Parser parser;
        Executor executor;

        public VirtualMachine()
        {
            state = new MachineState();
            parser = new Parser();
            executor = new Executor(state);
        }

        public bool run(string statement)
        {
            InstructionStructure instStructure = parser.parseStatement(statement);

            executor.processInstruction(instStructure);
            return true;
        }

        public bool run(string[] statements)
        {
            InstructionStructure instructionStructure;
            InstructionOutput instructionOutput;
            int currentStatementIndex = 0;
            string statement;

            while (currentStatementIndex < statements.Length && 
                   state.lastInstructionType != CommandType.Exit)
            {
                try
                {
                    statement = statements[currentStatementIndex];
                    if (string.IsNullOrWhiteSpace(statement))
                    {
                        currentStatementIndex++;
                        continue;
                    }

                    instructionStructure = parser.parseStatement(statement);
                    if(instructionStructure.Operation == Command.NONE)
                    {
                        currentStatementIndex++;
                        continue;
                    }

                    instructionOutput = executor.processInstruction(instructionStructure);                    

                    if (instructionOutput.jumpSteps != 0)
                    {
                        currentStatementIndex = currentStatementIndex + instructionOutput.jumpSteps;
                    }
                    else
                    {
                        currentStatementIndex++;
                    }
                } catch(Exception e)
                {
                    Console.WriteLine("Instruction failed \"" + statements[currentStatementIndex] + "\" Error:" + e.Message);
                    currentStatementIndex = currentStatementIndex + 1;
                }
            }
            return true;
        }
    }
}
