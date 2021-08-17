using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleAssembler
{
    // This enum denotes each message type to be used in ExceptionFactory
    enum ExceptionMessage {
        StackNotReady,
        InvalidNumberOfParams,
        InvalidOperand,
        IllegalJumpInstruction,
        InvalidCommand,
        InvalidMemoryLocation
    }

    // ExceptionFactory has one public static method that is responsible for creating an Exception type
    // with a custom message. The message is dependent on the ExceptionMessage enum.
    class ExceptionFactory
    {
        public static Exception getException(ExceptionMessage messageType)
        {
            string message = "";

            switch(messageType)
            {
                case ExceptionMessage.StackNotReady:
                    message = "Stack not ready to execute this instruction";
                    break;
                case ExceptionMessage.InvalidNumberOfParams:
                    message = "Invalid number of parameters";
                    break;
                case ExceptionMessage.InvalidOperand:
                    message = "Invalid operand";
                    break;
                case ExceptionMessage.IllegalJumpInstruction:
                    message = "Illegal Instruction: Last instruction was not a comparison";
                    break;
                case ExceptionMessage.InvalidCommand:
                    message = "Invalid command";
                    break;
                case ExceptionMessage.InvalidMemoryLocation:
                    message = "Illegal memory address";
                    break;
                default:
                    message = "Unknown error occured";
                    break;
            }

            return new Exception(message);
        }
    }
}
