using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleAssembler
{
    // InstructionOutput is returned by each method for the supported operations.
    // It is only used for jump instructions to communicate to VirtualMachine
    // to jump certain number of steps (instructions).
    class InstructionOutput
    {
        public int jumpSteps;

        public InstructionOutput()
        {
            jumpSteps = 0;
        }
    }

    // Executor - executes the command input using processInstruction(..) method
    // It takes InstructionStructure type object as an input and returns 
    // InstructionOutput object.
    // For each supported command, following type of signatures would be there:
    // InstructionOutput Command(InstructionStructure instructionStructure)
    //
    // TODO: I would like to make an improvement in next iteration such that
    // each command processing method is linked to a map rather than being called
    // using a switch-case.
    class Executor
    {
        MachineState state;

        public Executor(MachineState machineState)
        {
            state = machineState;
        }

        /*
         * add top two values on stack and push result on stack
         * numOperands = 0
         */
        InstructionOutput Add(InstructionStructure instructionStructure)
        {
            try
            {
                if(instructionStructure.NumberOfOperands != 0)
                {
                    throw ExceptionFactory.getException(ExceptionMessage.InvalidNumberOfParams);
                } else if(state.StackSize < 2)
                {
                    throw ExceptionFactory.getException(ExceptionMessage.StackNotReady);
                }

                int value1 = state.pop();
                int value2 = state.pop();
                state.push(value1 + value2);
            } catch(Exception e)
            {
                throw e;
            } finally
            {
                state.lastInstructionType = CommandType.Data;
            }
            return new InstructionOutput();
        }

        /*
         * multiply top two values on stack and push result on stack
         * numOperands = 0
         */
        InstructionOutput Mul(InstructionStructure instructionStructure)
        {
            try
            {
                if (instructionStructure.NumberOfOperands != 0)
                {
                    throw ExceptionFactory.getException(ExceptionMessage.InvalidNumberOfParams);
                } else if (state.StackSize < 2)
                {
                    throw ExceptionFactory.getException(ExceptionMessage.StackNotReady);
                }

                int value1 = state.pop();
                int value2 = state.pop();
                state.push(value1 * value2);
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                state.lastInstructionType = CommandType.Data;
            }
            return new InstructionOutput();
        }

        /*
         * Move from operand2 to operand1
         * numOperands = 2
         * operand1 = MemoryLocation
         * operand2 = MemoryLocation/Integer
         */
        InstructionOutput Mov(InstructionStructure instructionStructure)
        {
            try
            {
                if(instructionStructure.NumberOfOperands != 2)
                {
                    throw ExceptionFactory.getException(ExceptionMessage.InvalidNumberOfParams);
                } else if(instructionStructure.Operand1.type != OperandType.MemoryLocation)
                {
                    throw ExceptionFactory.getException(ExceptionMessage.InvalidOperand);
                }
                
                if( instructionStructure.Operand2.type != OperandType.Integer &&
                    instructionStructure.Operand2.type != OperandType.MemoryLocation)
                {
                    throw ExceptionFactory.getException(ExceptionMessage.InvalidOperand);
                }

                int value = instructionStructure.Operand2.type == OperandType.MemoryLocation ?
                            state.readMemory(instructionStructure.Operand2.value) : instructionStructure.Operand2.value;
                state.writeMemory(instructionStructure.Operand1.value, value);

            } catch( Exception e)
            {
                throw e;
            } finally
            {
                state.lastInstructionType = CommandType.Data;
            }
            return new InstructionOutput();
        }

        /*
         * Compare operand1 and operand2, and store result in state's comparisonResult
         * numOperands = 2
         * operand1 = MemoryLocation/Integer
         * operand2 = MemoryLocation/Integer
         */
        InstructionOutput Cmp(InstructionStructure instructionStructure)
        {
            try
            {
                if (instructionStructure.NumberOfOperands != 2)
                {
                    throw ExceptionFactory.getException(ExceptionMessage.InvalidNumberOfParams);
                }

                if ( (instructionStructure.Operand1.type != OperandType.Integer &&
                      instructionStructure.Operand1.type != OperandType.MemoryLocation) ||
                     (instructionStructure.Operand2.type != OperandType.Integer &&
                      instructionStructure.Operand2.type != OperandType.MemoryLocation))
                {
                    throw ExceptionFactory.getException(ExceptionMessage.InvalidOperand);
                }

                int value1 = instructionStructure.Operand1.type == OperandType.MemoryLocation ?
                            state.readMemory(instructionStructure.Operand1.value) : instructionStructure.Operand1.value;

                int value2 = instructionStructure.Operand2.type == OperandType.MemoryLocation ?
                            state.readMemory(instructionStructure.Operand2.value) : instructionStructure.Operand2.value;

                if(value1 < value2)
                {
                    state.comparisonResult = ComparisonResult.LessThan;
                } else if(value1 > value2)
                {
                    state.comparisonResult = ComparisonResult.GreaterThan;
                } else
                {
                    state.comparisonResult = ComparisonResult.Equal;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                state.lastInstructionType = CommandType.Comparison;
            }
            return new InstructionOutput();
        }

        /*
         * Jumps certain steps if memory location or integer is positive or previous 
         * comparison operation result was greater than
         * numOperands = 2 or 1
         * operand1 = MemoryLocation/Integer
         * operand2 = Integer
         */
        InstructionOutput Jpos(InstructionStructure instructionStructure)
        {
            InstructionOutput instructionOutput = new InstructionOutput();

            try
            {
                // only jump steps are provided based on previous comparion operation
                if(instructionStructure.NumberOfOperands == 1)
                {
                    if(state.lastInstructionType != CommandType.Comparison)
                    {
                        throw ExceptionFactory.getException(ExceptionMessage.IllegalJumpInstruction);
                    }

                    if(instructionStructure.Operand1.type != OperandType.Integer)
                    {
                        throw ExceptionFactory.getException(ExceptionMessage.InvalidOperand);
                    }

                    if (state.comparisonResult == ComparisonResult.GreaterThan)
                    {
                        instructionOutput.jumpSteps = instructionStructure.Operand1.value;
                    }
                    // only jump if first operand is positive
                } else if(instructionStructure.NumberOfOperands == 2)
                {
                    if (instructionStructure.Operand2.type != OperandType.Integer)
                    {
                        throw ExceptionFactory.getException(ExceptionMessage.InvalidOperand);
                    }

                    int value = instructionStructure.Operand1.type == OperandType.MemoryLocation ?
                                state.readMemory(instructionStructure.Operand1.value) : instructionStructure.Operand1.value;

                    if( value > 0)
                    {
                        instructionOutput.jumpSteps = instructionStructure.Operand2.value;
                    }
                } else
                {
                    throw ExceptionFactory.getException(ExceptionMessage.InvalidNumberOfParams);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                state.lastInstructionType = CommandType.Jump;
            }
            return instructionOutput;
        }

        /*
         * Jumps certain steps if memory location or integer is zero or previous 
         * comparison operation result was equal
         * numOperands = 2 or 1
         * operand1 = MemoryLocation/Integer
         * operand2 = Integer
         */
        InstructionOutput Jz(InstructionStructure instructionStructure)
        {
            InstructionOutput instructionOutput = new InstructionOutput();

            try
            {
                // only jump steps are provided based on previous comparion operation
                if (instructionStructure.NumberOfOperands == 1)
                {
                    if (state.lastInstructionType != CommandType.Comparison)
                    {
                        throw ExceptionFactory.getException(ExceptionMessage.IllegalJumpInstruction);
                    }

                    if (instructionStructure.Operand1.type != OperandType.Integer)
                    {
                        throw ExceptionFactory.getException(ExceptionMessage.InvalidOperand);
                    }

                    if (state.comparisonResult == ComparisonResult.Equal)
                    {
                        instructionOutput.jumpSteps = instructionStructure.Operand1.value;
                    }
                    // only jump if first operand is positive
                }
                else if (instructionStructure.NumberOfOperands == 2)
                {
                    if (instructionStructure.Operand2.type != OperandType.Integer)
                    {
                        throw ExceptionFactory.getException(ExceptionMessage.InvalidOperand);
                    }

                    int value = instructionStructure.Operand1.type == OperandType.MemoryLocation ?
                                state.readMemory(instructionStructure.Operand1.value) : instructionStructure.Operand1.value;

                    if (value == 0)
                    {
                        instructionOutput.jumpSteps = instructionStructure.Operand2.value;
                    }
                }
                else
                {
                    throw ExceptionFactory.getException(ExceptionMessage.InvalidNumberOfParams);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                state.lastInstructionType = CommandType.Jump;
            }
            return instructionOutput;
        }

        /*
         * Jumps certain steps if memory location or integer is not zero or previous 
         * comparison operation result was not equal
         * numOperands = 2 or 1
         * operand1 = MemoryLocation/Integer
         * operand2 = Integer
         */
        InstructionOutput Jnz(InstructionStructure instructionStructure)
        {
            InstructionOutput instructionOutput = new InstructionOutput();

            try
            {
                // only jump steps are provided based on previous comparion operation
                if (instructionStructure.NumberOfOperands == 1)
                {
                    if (state.lastInstructionType != CommandType.Comparison)
                    {
                        throw ExceptionFactory.getException(ExceptionMessage.IllegalJumpInstruction);
                    } else if (instructionStructure.Operand1.type != OperandType.Integer)
                    {
                        throw ExceptionFactory.getException(ExceptionMessage.InvalidOperand);
                    }

                    if (state.comparisonResult != ComparisonResult.Equal)
                    {
                        instructionOutput.jumpSteps = instructionStructure.Operand1.value;
                    }
                    // only jump if first operand is positive
                }
                else if (instructionStructure.NumberOfOperands == 2)
                {
                    if (instructionStructure.Operand2.type != OperandType.Integer)
                    {
                        throw ExceptionFactory.getException(ExceptionMessage.InvalidOperand);
                    }

                    int value = instructionStructure.Operand1.type == OperandType.MemoryLocation ?
                                state.readMemory(instructionStructure.Operand1.value) : instructionStructure.Operand1.value;

                    if (value != 0)
                    {
                        instructionOutput.jumpSteps = instructionStructure.Operand2.value;
                    }
                }
                else
                {
                    throw ExceptionFactory.getException(ExceptionMessage.InvalidNumberOfParams);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                state.lastInstructionType = CommandType.Jump;
            }
            return instructionOutput;
        }

        /*
         * Push value in operand1 on stack
         * numOperands = 1
         * operand1 = MemoryLocation/Integer
         */
        InstructionOutput Push(InstructionStructure instructionStructure)
        {
            try
            {
                if (instructionStructure.NumberOfOperands != 1)
                {
                    throw ExceptionFactory.getException(ExceptionMessage.InvalidNumberOfParams);
                }

                int value = instructionStructure.Operand1.type == OperandType.MemoryLocation ?
                            state.readMemory(instructionStructure.Operand1.value) : instructionStructure.Operand1.value;
                state.push(value);
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                state.lastInstructionType = CommandType.Data;
            }
            return new InstructionOutput();
        }

        /*
         * Pop value in operand1 from stack
         * numOperands = 1
         * operand1 = MemoryLocation
         */
        InstructionOutput Pop(InstructionStructure instructionStructure)
        {
            try
            {
                if (instructionStructure.NumberOfOperands != 1)
                {
                    throw ExceptionFactory.getException(ExceptionMessage.InvalidNumberOfParams);
                } 
                if (instructionStructure.Operand1.type != OperandType.MemoryLocation)
                {
                    throw ExceptionFactory.getException(ExceptionMessage.InvalidOperand);
                }

                int value = state.pop();
                state.writeMemory(instructionStructure.Operand1.value, value);
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                state.lastInstructionType = CommandType.Data;
            }
            return new InstructionOutput();
        }

        /*
         * Output first operand
         * numOperands = 1
         * operand1 = MemoryLocation/Integer
         */
        InstructionOutput Output(InstructionStructure instructionStructure)
        {
            try
            {
                if (instructionStructure.NumberOfOperands != 1)
                {                    
                    throw ExceptionFactory.getException(ExceptionMessage.InvalidNumberOfParams);
                }

                int value = instructionStructure.Operand1.type == OperandType.MemoryLocation ?
                            state.readMemory(instructionStructure.Operand1.value) : instructionStructure.Operand1.value;
                Console.WriteLine("Output " + value);
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                state.lastInstructionType = CommandType.Data;
            }
            return new InstructionOutput();
        }

        /*
         * Exit the program
         * numOperands = 0
         */
        InstructionOutput Exit(InstructionStructure instructionStructure)
        {
            state.lastInstructionType = CommandType.Exit;
            return new InstructionOutput();
        }

        InstructionOutput runInstruction(InstructionStructure instructionStructure)
        {
            switch (instructionStructure.Operation)
            {
                case Command.ADD: return Add(instructionStructure);
                case Command.MUL: return Mul(instructionStructure);
                case Command.MOV: return Mov(instructionStructure);
                case Command.CMP: return Cmp(instructionStructure);
                case Command.PUSH: return Push(instructionStructure);
                case Command.POP: return Pop(instructionStructure);
                case Command.JPOS: return Jpos(instructionStructure);
                case Command.JZ: return Jz(instructionStructure);
                case Command.JNZ: return Jnz(instructionStructure);
                case Command.OUTPUT: return Output(instructionStructure);
                case Command.EXIT: return Exit(instructionStructure);
                default: throw ExceptionFactory.getException(ExceptionMessage.InvalidCommand);
            }
        }

        public InstructionOutput processInstruction(InstructionStructure instructionStructure)
        {
            return runInstruction(instructionStructure);
        }
    }
}
