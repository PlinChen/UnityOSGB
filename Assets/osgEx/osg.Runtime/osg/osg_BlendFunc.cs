﻿using System.IO;

namespace osgEx
{
    public class osg_BlendFunc : osg_StateAttribute
    {
        protected override void read(BinaryReader reader, osg_Reader owner)
        {
            base.read(reader, owner);
            //_source_factor
            reader.ReadUInt32();
            //_source_factor_alpha
            reader.ReadUInt32();
            //_destination_factor
            reader.ReadUInt32();
            //_destination_factor_alpha
            reader.ReadUInt32();
        }
    }
}
