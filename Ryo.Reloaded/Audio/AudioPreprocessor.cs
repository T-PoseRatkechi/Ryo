using Ryo.Interfaces.Structs;
using Ryo.Reloaded.Audio.Models.Containers;

namespace Ryo.Reloaded.Audio;

internal class AudioPreprocessor
{
    private readonly List<Preprocessor> preprocessors = new();

    public void AddPreprocessor(string name, Func<AudioInfo, AudioInfo> func)
        => this.preprocessors.Add(new(name, func));

    public void Preprocess(CueContainer audio)
    {
        // TODO: Rethink where preprocessing is handled since
        // audio containers should be immutable.

        //foreach (var processor in this.preprocessors)
        //{
        //    Log.Debug($"Running preprocessor: {processor.Name}\nFile: {audio.AudioFile}");
        //    var result = processor.Func.Invoke(new AudioInfo()
        //    {
        //        AudioFile = audio.AudioFile,
        //        Format = audio.Format,
        //        SampleRate = audio.SampleRate,
        //        NumChannels = audio.NumChannels,
        //        Volume = audio.Volume,
        //        Key = audio.Key,
        //        Tags = audio.Tags,
        //    });

        //    audio.AudioFile = result.AudioFile;
        //    audio.Format = result.Format;
        //    audio.SampleRate = result.SampleRate;
        //    audio.NumChannels = result.NumChannels;
        //    audio.Volume = result.Volume;
        //    audio.Key = result.Key;
        //    audio.Tags = result.Tags;
        //}
    }

    private record Preprocessor(string Name, Func<AudioInfo, AudioInfo> Func);
}
