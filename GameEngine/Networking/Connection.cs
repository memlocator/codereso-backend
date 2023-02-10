using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.WebSockets;

namespace GameEngine.Networking
{
    
    internal class Connection
    {
        List<bool> replicatedObjects;
    }
}

//class Connection
//{
//    List<bool> objectsreplicated;

//    bool isReplicated(int id)
//    {
//        return id >= 0 && id < objectsreplicated.Count && objectsreplicated[id];
//    }

//    void replicate(int id)
//    {
//        if (isReplicated(id)) return;

//        // expand list if too small

//        objectsreplicated[id] = true;
//        replicate();
//    }

//    void update(int id)
//    {
//        if (!isReplicated(id)) return;

//        updateobject();
//    }

//    void cleanup(int id)
//    {
//        if (!isReplicated(id)) return;

//        cleanup();
//    }

//}