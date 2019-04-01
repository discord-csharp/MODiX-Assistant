/*

ooo        ooooo   .oooooo.   oooooooooo.    o8o  ooooooo  ooooo 
`88.       .888'  d8P'  `Y8b  `888'   `Y8b   `"'   `8888    d8'  
 888b     d'888  888      888  888      888 oooo     Y888..8P    
 8 Y88. .P  888  888      888  888      888 `888      `8888'     
 8  `888'   888  888      888  888      888  888     .8PY888.    
 8    Y     888  `88b    d88'  888     d88'  888    d8'  `888b   
o8o        o888o  `Y8bood8P'  o888bood8P'   o888o o888o  o88888o 

*/

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ModixAssistant
{
    public interface IBot
    {
        Task Ready();
    }
}
