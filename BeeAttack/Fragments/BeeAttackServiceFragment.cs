using Android.App;
using Android.OS;

namespace BeeAttack.Fragments
{
    public class BeeAttackServiceFragment : Fragment
    {
        public Services.BeeAttackService Service { get; private set; }

        public BeeAttackServiceFragment(Services.BeeAttackService service)
        {
            Service = service;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            RetainInstance = true;
        }
    }
}