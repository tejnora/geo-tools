using System;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using System.Windows;

namespace GeoBase.Utils
{
    public class Registry
    {
        public enum SubKey
        {
            kRoot,
            kCommandLine,
            kEnvironment,
            kUser,
            kCurrentUser,
            kAllUsers,
            kSystem, // currentUser
            kProgram,
            kConfig,
            kAllUsersLocal
        };
        private bool mAlreadyLoaded;
        private bool mAllowCurrentUserConfig = true;
        private string mCurrentUser;
        private Dictionary<string, ProgramOption> mEntry2Option;
        private Dictionary<string, ProgramOption> mProgramEntry2Option;
        private Dictionary<string, ProgramOption> mConfig2Option;

        private const string _magicHeader = "Config";
        private Registry()
        {
            mCurrentUser = Environment.UserName;
            SuppressRegistry = false;
        }
        ~Registry()
        {
            save();
        }
        public void Save()
        {
            save();
        }
        public bool SuppressRegistry
        {
            get; set;
        }
        public string CurrentUser
        {
            set { mCurrentUser = value; }
            get { return mCurrentUser; }
        }

        public string getCurrentUserName()
        {
            return Environment.UserName;
        }

        public ProgramOption getEntry(string aName)
        {
            if (aName.StartsWith(subKey(SubKey.kEnvironment)))
            {
                return getEntry(SubKey.kEnvironment, aName.Substring(subKey(SubKey.kEnvironment).Length));
            }
            if (aName.StartsWith(subKey(SubKey.kCommandLine)))
            {
                return getEntry(SubKey.kCommandLine, aName.Substring(subKey(SubKey.kCommandLine).Length));
            }
            if (aName.StartsWith(subKey(SubKey.kSystem)))
            {
                //            return getSystemEntry(aName.Substring(subKey(SubKey.kSystem).Length));
            }
            if (aName.StartsWith(subKey(SubKey.kProgram)))
            {
                return getEntry(SubKey.kProgram, aName.Substring(subKey(SubKey.kProgram).Length));
            }
            if (aName.StartsWith(subKey(SubKey.kConfig)))
            {
                return getEntry(SubKey.kConfig, aName.Substring(subKey(SubKey.kConfig).Length));
            }

            if (!mAlreadyLoaded) load();
            if (aName.StartsWith(subKey(SubKey.kCurrentUser)) && mAllowCurrentUserConfig)
            {
                string entryName = aName.Substring(subKey(SubKey.kCurrentUser).Length);
                string name = mCurrentUser + "\\" + entryName;
                if (mEntry2Option.ContainsKey(name))
                {
                    return mEntry2Option[name];
                }
                else
                {
                    aName = subKey(SubKey.kAllUsers) + entryName;
                }
            }
            if (!mEntry2Option.ContainsKey(aName))
            {
                return new ProgramOption();
            }

            return mEntry2Option[aName];
        }

        public ProgramOption getEntry(SubKey aSubKey, String aName)
        {
            if (aSubKey == SubKey.kSystem)
            {
                //         return getSystemEntry(aName);
            }
            if (aSubKey == SubKey.kCommandLine)
            {
                //         return GET_SINGLETON_IFACE(CommandLineOptions,CommandLineOptionsIface)->getOption(aName);
            }
            if (aSubKey == SubKey.kProgram)
            {
                if (mProgramEntry2Option.ContainsKey(aName))
                    return mProgramEntry2Option[aName];
                else
                    return new ProgramOption();
            }
            if (aSubKey == SubKey.kConfig)
            {
                if (!mAlreadyLoaded) load();
                if (mConfig2Option.ContainsKey(aName))
                    return mConfig2Option[aName];
                else
                    return new ProgramOption();
            }
            if (aSubKey == SubKey.kEnvironment)
            {
                return new ProgramOption();
            }
            return getEntry(subKey(aSubKey) + aName);
        }

        public void setEntry(string aName, ProgramOption aOption)
        {
            if (aName.StartsWith(subKey(SubKey.kEnvironment)))
            {
                setEntry(SubKey.kEnvironment, aName.Substring(subKey(SubKey.kEnvironment).Length), aOption);
                return;
            }
            if (aName.StartsWith(subKey(SubKey.kProgram)))
            {
                setEntry(SubKey.kProgram, aName.Substring(subKey(SubKey.kProgram).Length), aOption);
                return;
            }
            if (aName.StartsWith(subKey(SubKey.kConfig)))
            {
                setEntry(SubKey.kConfig, aName.Substring(subKey(SubKey.kConfig).Length), aOption);
                return;
            }
            if (aName.StartsWith(subKey(SubKey.kCommandLine)))
            {
                System.Diagnostics.Debug.Fail("NEVER_GET_HERE");
                return;
            }
            if (!mAlreadyLoaded) load();
            if (aName.StartsWith(subKey(SubKey.kCurrentUser)) && mAllowCurrentUserConfig)
            {
                aName = mCurrentUser + aName.Substring(subKey(SubKey.kCurrentUser).Length - 1);
            }
            // the entry must be unique
            mEntry2Option.Remove(aName);
            if (aOption.isSet())
            {
                mEntry2Option[aName] = aOption;
            }
        }

        public void setEntry(SubKey aSubKey, String aName, ProgramOption aOption)
        {
            if (aSubKey == SubKey.kProgram) 
            {
                mProgramEntry2Option.Remove(aName);
                if (aOption.isSet())
                {
                    mProgramEntry2Option[aName] = aOption;
                }
                return;
            }
            if (aSubKey == SubKey.kConfig)
            {
                mProgramEntry2Option.Remove(aName);
                if (aOption.isSet())
                {
                    mConfig2Option[aName] = aOption;
                }
                return;
            }
            if (aSubKey == SubKey.kEnvironment)
            {
            }
            setEntry(subKey(aSubKey) + aName, aOption);
        }

        private void load()
        {
            mEntry2Option = new Dictionary<string, ProgramOption>();
            if(SuppressRegistry)
            {
                mAlreadyLoaded = true;
                return;
            }
            String appDir = AppDomain.CurrentDomain.BaseDirectory;
            String regName = string.Format("{0}.cfg",
                                           Path.GetFileNameWithoutExtension(Assembly.GetEntryAssembly().Location));
            String configPath = appDir + regName;
            if (File.Exists(configPath))
            {
                using (BinaryReader reader = new BinaryReader(File.Open(configPath, FileMode.Open)))
                {
                    if (reader.ReadString() != _magicHeader)
                        return;
                    UInt32 count = reader.ReadUInt32();
                    while (count > 0)
                    {
                        ProgramOption.Type type = (ProgramOption.Type)reader.ReadByte();
                        ProgramOption po = new ProgramOption();
                        string name = reader.ReadString();
                        switch (type)
                        {
                            case ProgramOption.Type.tBool:
                                {
                                    po = new ProgramOption(reader.ReadBoolean());
                                } break;
                            case ProgramOption.Type.tDouble:
                                {
                                    po = new ProgramOption(reader.ReadDouble());
                                } break;
                            case ProgramOption.Type.tInt:
                                {
                                    po = new ProgramOption(reader.ReadInt32());
                                } break;
                            case ProgramOption.Type.tString:
                                {
                                    po = new ProgramOption(reader.ReadString());
                                } break;
                            case ProgramOption.Type.tPoint:
                                {
                                    Double x = reader.ReadDouble();
                                    Double y = reader.ReadDouble();
                                    po = new ProgramOption(new Point(x, y));
                                } break;
                            default:
                                {
                                    System.Diagnostics.Debug.Assert(false);
                                } break;
                        }
                        count--;
                        mEntry2Option.Add(name, po);
                    }
                }
            }
            mAlreadyLoaded = true;
        }

        private void save()
        {
            if(SuppressRegistry)return;
            String appDir = AppDomain.CurrentDomain.BaseDirectory;
            String regName = string.Format("{0}.cfg",
                               Path.GetFileNameWithoutExtension(Assembly.GetEntryAssembly().Location));
            String configPath = appDir + regName;
            using (BinaryWriter writer = new BinaryWriter(File.Open(configPath, FileMode.Create)))
            {
                writer.Write(_magicHeader);
                writer.Write(mEntry2Option.Count);
                foreach (KeyValuePair<string, ProgramOption> val in mEntry2Option)
                {
                    writer.Write((byte)val.Value.getType());
                    writer.Write(val.Key);
                    switch (val.Value.getType())
                    {
                        case ProgramOption.Type.tBool:
                            {
                                writer.Write(val.Value.getBool());
                            } break;
                        case ProgramOption.Type.tDouble:
                            {
                                writer.Write(val.Value.getDouble());
                            } break;
                        case ProgramOption.Type.tInt:
                            {
                                writer.Write(val.Value.getInt());
                            } break;
                        case ProgramOption.Type.tString:
                            {
                                writer.Write(val.Value.getString());
                            } break;
                        case ProgramOption.Type.tPoint:
                            {
                                writer.Write(val.Value.getPoint().X);
                                writer.Write(val.Value.getPoint().Y);
                            } break;
                        default:
                            {
                                System.Diagnostics.Debug.Assert(false);
                            } break;
                    }
                }
            }
        }

        private string subKey(SubKey aSubKey)
        {
            if (aSubKey == SubKey.kRoot)
            {
                return string.Empty;
            }
            else if (aSubKey == SubKey.kCommandLine)
            {
                return "CommandLine\\";
            }
            else if (aSubKey == SubKey.kEnvironment)
            {
                return "Environment\\";
            }
            else if (aSubKey == SubKey.kUser)
            {
                return "User\\";
            }
            else if (aSubKey == SubKey.kCurrentUser)
            {
                return "CurrentUser\\";
            }
            else if (aSubKey == SubKey.kAllUsers)
            {
                return "AllUsers\\";
            }
            else if (aSubKey == SubKey.kSystem)
            {
                return "System\\";
            }
            else if (aSubKey == SubKey.kProgram)
            {
                return "Program\\";
            }
            else if (aSubKey == SubKey.kConfig)
            {
                return "Config\\";
            }
            else if (aSubKey == SubKey.kAllUsersLocal)
            {
                return "AllUsersLocal\\";
            }
            System.Diagnostics.Debug.Fail("NEVER_GET_HERE");
            return string.Empty;
        }
    }
}
