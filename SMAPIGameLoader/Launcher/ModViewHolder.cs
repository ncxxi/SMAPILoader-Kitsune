using Android.Content;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.Widget;
using AndroidX.RecyclerView.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Android.Provider.Contacts.Intents;

namespace SMAPIGameLoader.Launcher;


internal class ModViewHolder : RecyclerView.ViewHolder
{
    private readonly TextView _name;
    private readonly TextView _path;
    private readonly TextView _version;
    private readonly SwitchCompat _toggle;
    private readonly Action<ModItemView> _onToggled;
    private readonly Action<ModItemView, View> _onClicked;
    private ModItemView? _mod;
    private bool _binding;

    private ModViewHolder(View root, TextView name, TextView path, TextView version,
                          SwitchCompat toggle, Action<ModItemView> onToggled,
                          Action<ModItemView, View> onClicked) : base(root)
    {
        _name = name;
        _path = path;
        _version = version;
        _toggle = toggle;
        _onToggled = onToggled;
        _onClicked = onClicked;

        _toggle.CheckedChange += (s, e) =>
        {
            if (_binding || _mod == null) return;
            _mod.Enabled = e.IsChecked;
            ApplyEnabledStyle(_mod.Enabled);
            _onToggled(_mod);
        };

        root.Click += (s, e) =>
        {
            if (_mod != null) _onClicked(_mod, root);
        };
    }

    public static ModViewHolder Create(Context context,
                                       Action<ModItemView> onToggled,
                                       Action<ModItemView, View> onClicked)
    {
        var root = new LinearLayout(context) { Orientation = Orientation.Horizontal };
        root.SetPadding(Ui.Dp(context, 16), Ui.Dp(context, 12),
                        Ui.Dp(context, 16), Ui.Dp(context, 12));
        root.LayoutParameters = new RecyclerView.LayoutParams(
            ViewGroup.LayoutParams.MatchParent,
            ViewGroup.LayoutParams.WrapContent);
        root.SetGravity(GravityFlags.CenterVertical);

        // ซ้าย: ชื่อ + path
        var left = new LinearLayout(context) { Orientation = Orientation.Vertical };
        var name = new TextView(context);
        name.SetTextSize(Android.Util.ComplexUnitType.Sp, 16);
        name.SetTypeface(null, Android.Graphics.TypefaceStyle.Bold);
        var path = new TextView(context);
        path.SetTextSize(Android.Util.ComplexUnitType.Sp, 12);
        path.Alpha = 0.6f;
        left.AddView(name);
        left.AddView(path);
        root.AddView(left, new LinearLayout.LayoutParams(0, ViewGroup.LayoutParams.WrapContent, 1f));

        // ขวา: version + switch
        var version = new TextView(context);
        version.SetTextSize(Android.Util.ComplexUnitType.Sp, 12);
        version.Alpha = 0.6f;
        var toggle = new SwitchCompat(context);
        root.AddView(version);
        root.AddView(toggle);

        return new ModViewHolder(root, name, path, version, toggle, onToggled, onClicked);
    }

    public void Bind(ModItemView mod)
    {
        _mod = mod;
        _name.Text = mod.NameUIText;
        _path.Text = mod.FolderPathUIText;
        _version.Text = mod.VersionUIText;

        _binding = true;
        _toggle.Checked = mod.Enabled;
        _binding = false;

        ApplyEnabledStyle(mod.Enabled);
    }

    private void ApplyEnabledStyle(bool enabled)
    {
        float alpha = enabled ? 1f : 0.5f;
        _name.Alpha = alpha;
        _path.Alpha = enabled ? 0.6f : 0.3f;
    }
}
