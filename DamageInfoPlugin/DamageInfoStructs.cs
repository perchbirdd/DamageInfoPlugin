using System;
using System.Numerics;
using Dalamud.Game.Gui.FlyText;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace DamageInfoPlugin;

public unsafe struct CastbarInfo
{
    public AtkUnitBase* unitBase;
    public AtkImageNode* gauge;
    public AtkImageNode* bg;

    public bool Valid()
    {
        return unitBase != null && gauge != null && bg != null;
    }

    public void ResetIfValid()
    {
        if (Valid())
            Reset();
    }

    public void Reset()
    {
        gauge->AtkResNode.Color.R = 0xFF;
        gauge->AtkResNode.Color.G = 0xFF;
        gauge->AtkResNode.Color.B = 0xFF;
        gauge->AtkResNode.Color.A = 0xFF;

        bg->AtkResNode.Color.R = 0xFF;
        bg->AtkResNode.Color.G = 0xFF;
        bg->AtkResNode.Color.B = 0xFF;
        bg->AtkResNode.Color.A = 0xFF;
    }

    public void Color(Vector4 gaugeColor, Vector4 bgColor)
    {
        gauge->AtkResNode.Color.R = (byte)(gaugeColor.X * 255);
        gauge->AtkResNode.Color.G = (byte)(gaugeColor.Y * 255);
        gauge->AtkResNode.Color.B = (byte)(gaugeColor.Z * 255);
        gauge->AtkResNode.Color.A = (byte)(gaugeColor.W * 255);
        
        bg->AtkResNode.Color.R = (byte)(bgColor.X * 255);
        bg->AtkResNode.Color.G = (byte)(bgColor.Y * 255);
        bg->AtkResNode.Color.B = (byte)(bgColor.Z * 255);
        bg->AtkResNode.Color.A = (byte)(bgColor.W * 255);
    }

    public static bool operator !=(CastbarInfo cb1, CastbarInfo cb2) => !cb1.Equals(cb2);
    public static bool operator ==(CastbarInfo cb1, CastbarInfo cb2) => cb1.Equals(cb2);
    public bool Equals(CastbarInfo other) => unitBase == other.unitBase && gauge == other.gauge && bg == other.bg;
    public override bool Equals(object obj) => obj is CastbarInfo other && Equals(other);
    public override int GetHashCode() => HashCode.Combine(unchecked((int)(long)unitBase), unchecked((int)(long)gauge), unchecked((int)(long)bg));
}

public static class EffectExtensions
{
    public static ActionEffectType GetEffectType(this ActionEffectHandler.Effect effect) => (ActionEffectType)effect.Type;

    public static AttackType GetAttackType(this ActionEffectHandler.Effect effect) => (AttackType)(byte)(effect.Param1 & 0xF);
    
    public static DamageType GetDamageType(this ActionEffectHandler.Effect effect) => effect.GetAttackType().ToDamageType();

    public static uint GetDamage(this ActionEffectHandler.Effect effect) => effect.Value + ((uint)ushort.MaxValue + 1) * effect.Param3;

    public static string ToText(this ActionEffectHandler.Effect effect)
    {
        var attackType = effect.GetAttackType();
        return
            $"Type: {effect.GetEffectType()}, p0: {effect.Param0:D3}, p1: {effect.Param1:D3}, p2: {effect.Param2:D3} 0x{effect.Param2:X2} '{Convert.ToString(effect.Param2, 2).PadLeft(8, '0')}', mult: {effect.Param3:D3}, flags: {effect.Param4:D3} | {Convert.ToString(effect.Param4, 2).PadLeft(8, '0')}, value: {effect.Value:D6} ATTACK TYPE: {attackType} DAMAGE TYPE: {attackType.ToDamageType()}";
    }
}

public struct ActionEffectInfo
{
    public ActionStep step;
    public ulong tick;
        
    public uint actionId;
    public ActionEffectType type;
    public DamageType damageType;
    public FlyTextKind kind;
    public uint sourceId;
    public ulong targetId;
    public uint value;
    public PositionalState positionalState;

    public bool Equals(ActionEffectInfo other) => step == other.step && tick == other.tick && actionId == other.actionId && type == other.type && damageType == other.damageType && kind == other.kind && sourceId == other.sourceId && targetId == other.targetId && value == other.value && positionalState == other.positionalState;
    public override bool Equals(object obj) => obj is ActionEffectInfo other && Equals(other);
    public override int GetHashCode()
    {
        var hashCode = new HashCode();
        hashCode.Add((int)step);
        hashCode.Add(tick);
        hashCode.Add(actionId);
        hashCode.Add((int)type);
        hashCode.Add(damageType);
        hashCode.Add((int)kind);
        hashCode.Add(sourceId);
        hashCode.Add(targetId);
        hashCode.Add(value);
        hashCode.Add((int)positionalState);
        return hashCode.ToHashCode();
    }

    public override string ToString() => $"{nameof(step)}: {step}, {nameof(tick)}: {tick}, {nameof(actionId)}: {actionId}, {nameof(type)}: {type}, {nameof(damageType)}: {damageType}, {nameof(kind)}: {kind}, {nameof(sourceId)}: {sourceId}, {nameof(targetId)}: {targetId}, {nameof(value)}: {value}, {nameof(positionalState)}: {positionalState}";
}