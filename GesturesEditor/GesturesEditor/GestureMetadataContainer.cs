using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace GesturesEditor
{

    public class GestureMetadataContainer
    {
        public int Id { get; set; }
        public String Name { get; set; }
        public ArrayList GesturesMetadata { get; set; }

        public GestureMetadataContainer(int id, String name, ArrayList gesturesMetadata)
        {
            Id = id;
            Name = name;
            GesturesMetadata = gesturesMetadata;
        }
    }
}
