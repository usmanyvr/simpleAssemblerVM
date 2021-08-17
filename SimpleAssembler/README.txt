
Virtual Machine Design
----------------------

Following is a basic layout of the design of Virtual Machine.

+---------------------------+                                                                   
|                           |                                                                   
|     VirtualMachine        |                                         +------------------------+
|                           |                                         |                        |
|                           |                                         |                        |
|---------------------------|                                         |  8 Memory locations    |
|        MachineState       |-----------------------------------------|  m1 .... m8            |
|                           |                                         |------------------------|
|-------------------------- |                                         |  Stack which can be    |
|        Parser             |-----------------|                       |  used to push and pop  |
|                           |                 |                       |  data (intgeger)       |
|-------------------------- |                 |                       |                        |
|        Executor           |                 |                       +------------------------+
|                           |                 |                                                 
+---------------------------+                 |                                                 
          +                         +---------------------+                                     
          |                         | Parser class parses |                                     
          |                         | each instruction and|                                     
          |                         | separates out       |                                     
          |                         | command, operand1   |                                     
          |                         | and operand2.       |                                     
+--------------------------+        | After processing it |                                     
|  Executor is a class that         | returns object of   |                                     
|  contains capability     |        | InstructionStructure|                                     
|  to run each command and |        |                     |                                     
|  update machine state    |        +---------------------+                                     
|  accordingly.            |                                                                    
|  Input is object of      |                                                                    
|  InstructionStructure    |                                                                    
+--------------------------+                                                                    

- VirtualMachine class basically "has" these different objects instantiated as it's members. 
- It takes input as array of strings with each string representing a line of semicolon
  delimited instruction. It also has an overloaded run(...) method which can process a single
  instruction.

TIME IT TOOK: It took about more than 12 hours to build this.

Package also includes Factorial.asm file which calculates factorial of a given number.