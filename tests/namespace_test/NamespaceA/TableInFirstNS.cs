// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

namespace NamespaceA
{

using global::System;
using global::FlatBuffers;

public struct TableInFirstNS : IFlatbufferObject
{
  private Table __p;
  public ByteBuffer ByteBuffer { get { return __p.bb; } }
  public static TableInFirstNS GetRootAsTableInFirstNS(ByteBuffer _bb) { return GetRootAsTableInFirstNS(_bb, new TableInFirstNS()); }
  public static TableInFirstNS GetRootAsTableInFirstNS(ByteBuffer _bb, TableInFirstNS obj) { FlatBufferConstants.FLATBUFFERS_1_11_1(); return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public void __init(int _i, ByteBuffer _bb) { __p.bb_pos = _i; __p.bb = _bb; }
  public TableInFirstNS __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

  public NamespaceA.NamespaceB.TableInNestedNS? FooTable { get { int o = __p.__offset(4); return o != 0 ? (NamespaceA.NamespaceB.TableInNestedNS?)(new NamespaceA.NamespaceB.TableInNestedNS()).__assign(__p.__indirect(o + __p.bb_pos), __p.bb) : null; } }
  public NamespaceA.NamespaceB.EnumInNestedNS FooEnum { get { int o = __p.__offset(6); return o != 0 ? (NamespaceA.NamespaceB.EnumInNestedNS)__p.bb.GetSbyte(o + __p.bb_pos) : NamespaceA.NamespaceB.EnumInNestedNS.A; } }
  public bool MutateFooEnum(NamespaceA.NamespaceB.EnumInNestedNS foo_enum) { int o = __p.__offset(6); if (o != 0) { __p.bb.PutSbyte(o + __p.bb_pos, (sbyte)foo_enum); return true; } else { return false; } }
  public NamespaceA.NamespaceB.StructInNestedNS? FooStruct { get { int o = __p.__offset(8); return o != 0 ? (NamespaceA.NamespaceB.StructInNestedNS?)(new NamespaceA.NamespaceB.StructInNestedNS()).__assign(o + __p.bb_pos, __p.bb) : null; } }

  public static void StartTableInFirstNS(FlatBufferBuilder builder) { builder.StartTable(3); }
  public static void AddFooTable(FlatBufferBuilder builder, Offset<NamespaceA.NamespaceB.TableInNestedNS> fooTableOffset) { builder.AddOffset(0, fooTableOffset.Value, 0); }
  public static void AddFooEnum(FlatBufferBuilder builder, NamespaceA.NamespaceB.EnumInNestedNS fooEnum) { builder.AddSbyte(1, (sbyte)fooEnum, 0); }
  public static void AddFooStruct(FlatBufferBuilder builder, Offset<NamespaceA.NamespaceB.StructInNestedNS> fooStructOffset) { builder.AddStruct(2, fooStructOffset.Value, 0); }
  public static Offset<TableInFirstNS> EndTableInFirstNS(FlatBufferBuilder builder) {
    int o = builder.EndTable();
    return new Offset<TableInFirstNS>(o);
  }
};


}
