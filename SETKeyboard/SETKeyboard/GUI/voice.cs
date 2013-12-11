using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Media;

namespace Voice
{
    class Synthesis
    {
        public static string Speak(string consoleText)
        {
            uint language = 0; // US English
            string module = "Voice Synthesis (Nuance*)";
            PXCMSession session;
            PXCMSession.CreateInstance(out session);
            PXCMSession.ImplDesc desc = new PXCMSession.ImplDesc();
            desc.friendlyName.set(module);
            desc.cuids[0] = PXCMVoiceSynthesis.CUID;
            PXCMVoiceSynthesis vsynth;
            session.CreateImpl<PXCMVoiceSynthesis>(ref desc, PXCMVoiceSynthesis.CUID, out vsynth);
            PXCMVoiceSynthesis.ProfileInfo pinfo;
            vsynth.QueryProfile(language, out pinfo);
            vsynth.SetProfile(ref pinfo);
            int tid = 0;
            vsynth.QueueSentence(consoleText, out tid);
            Output output = new Output(pinfo.outputs.info);

            while (true)
            {
                PXCMAudio sample;
                PXCMScheduler.SyncPoint sp;
                if (vsynth.ProcessAudioAsync(tid, out sample, out sp) < pxcmStatus.PXCM_STATUS_NO_ERROR) break;
                pxcmStatus sts = sp.Synchronize(); //
                output.RenderAudio(sample);
                sample.Dispose();
                sp.Dispose();
                if (sts < pxcmStatus.PXCM_STATUS_NO_ERROR) break;
            }
            
            output.Close();
            vsynth.Dispose();
            session.Dispose();
            return null;
        }

        public static string voice_synth(string consoleText)
        {
            uint language = 0; // US English
            PXCMSession session;
            PXCMSession.CreateInstance(out session);

            PXCMSession.ImplDesc desc = new PXCMSession.ImplDesc();
            desc.cuids[0] = PXCMVoiceSynthesis.CUID;
            PXCMVoiceSynthesis vsynth;
            session.CreateImpl<PXCMVoiceSynthesis>(ref desc, PXCMVoiceSynthesis.CUID, out vsynth);
            PXCMVoiceSynthesis.ProfileInfo pinfo;
            vsynth.QueryProfile(language, out pinfo);
            vsynth.SetProfile(ref pinfo);

            int tid = 0;
            vsynth.QueueSentence(consoleText, out tid);
            Output output = new Output(pinfo.outputs.info);
            while (true)
            {
                PXCMAudio sample;
                PXCMScheduler.SyncPoint sp;

                if (vsynth.ProcessAudioAsync(tid, out sample, out sp) < pxcmStatus.PXCM_STATUS_NO_ERROR) break;
                pxcmStatus sts = sp.Synchronize();
                output.RenderAudio(sample);
                sample.Dispose();
                sp.Dispose();
                if (sts < pxcmStatus.PXCM_STATUS_NO_ERROR) break;
            }
            output.Close();

            vsynth.Dispose();
            session.Dispose();
            return null;
        }
    }

	class Output
	{
		private MemoryStream mem_stream;
		private BinaryWriter binary_writer;

		public Output(PXCMAudio.AudioInfo ainfo)
		{
			mem_stream = new MemoryStream();
			
            binary_writer = new BinaryWriter(mem_stream);
			binary_writer.Write((int)0x46464952);
			binary_writer.Write((int)0);  
			binary_writer.Write((int)0x45564157); 
			binary_writer.Write((int)0x20746d66); 
			binary_writer.Write((int)0x12);
			binary_writer.Write((short)1);
			binary_writer.Write((short)ainfo.nchannels); 
			binary_writer.Write((int)ainfo.sampleRate); 
			binary_writer.Write((int)(ainfo.sampleRate * 2 * ainfo.nchannels));
			binary_writer.Write((short)(ainfo.nchannels * 2));   
			binary_writer.Write((short)16);        
			binary_writer.Write((short)0);         
			binary_writer.Write((int)0x61746164);  
			binary_writer.Write((int)0);           
		}

		public void RenderAudio(PXCMAudio audio)
		{
			PXCMAudio.AudioData adata;
			audio.AcquireAccess(PXCMAudio.Access.ACCESS_READ, PXCMAudio.AudioFormat.AUDIO_FORMAT_PCM, out adata);
			binary_writer.Write(adata.ToByteArray());
			audio.ReleaseAccess(ref adata);
		}

		public void Close()
		{
			long position = binary_writer.Seek(0, SeekOrigin.Current);
			binary_writer.Seek(0x2a, SeekOrigin.Begin);
			binary_writer.Write((int)(position - 46));
			binary_writer.Seek(0x04, SeekOrigin.Begin);
			binary_writer.Write((int)(position - 8));
			binary_writer.Seek(0, SeekOrigin.Begin);
			SoundPlayer sp = new SoundPlayer(mem_stream);
			sp.PlaySync();
			sp.Dispose();
			binary_writer.Close();
			mem_stream.Close();
		}
	}
}