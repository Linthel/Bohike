using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bohike.Managers
{
    public class SoundManager
    {
        private Song _song;
        public List<SoundEffect> _soundEffects;

        public SoundManager(Song song, List<SoundEffect> soundEffects)
        {
            _song = song;
            _soundEffects = soundEffects;
        }

        public void PlayMusic()
        {
            MediaPlayer.Play(_song);
        }

        public void PlaySoundEffect(int i)
        {
            _soundEffects[i].Play();
        }

        public SoundEffectInstance CreateInstance(int i)
        {
            return _soundEffects[i].CreateInstance();
        }
    }
}
