/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 *  
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *  
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
 * GNU General Public License for more details.
 *  
 * You should have received a copy of the GNU General Public License
 * along with this program. If not, see <http://www.gnu.org/licenses/>.
 */
using NAudio.Wave;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VAIS.ASR.Lib;

namespace Inventec.VoiceCommand
{
    public class VoiceCommandProcessor
    {
        List<CommandTypeADO> listAICommandActionControlConfig;

        /// <summary>
        /// Microphone chunks that haven't yet been processed at all.
        /// </summary>
        private readonly BlockingCollection<ProcessCommandADO> _microphoneBuffer = new BlockingCollection<ProcessCommandADO>();

        /// <summary>
        /// Chunks that have been sent to Cloud Speech, but not yet finalized.
        /// </summary>
        private readonly LinkedList<ProcessCommandADO> _processingBuffer = new LinkedList<ProcessCommandADO>();

        /// <summary>
        /// The start time of the processing buffer, in relation to the start of the stream.
        /// </summary>
        private DateTime _processingBufferStart;

        /// <summary>
        /// The deadline for when we should stop the current stream.
        /// </summary>
        private DateTime _rpcStreamDeadline;

        private readonly BlockingCollection<ResultCommandADO> _apiResultBuffer = new BlockingCollection<ResultCommandADO>();
        private BlockingCollection<float> _maxEmptyBuffer = new BlockingCollection<float>();
        private readonly BlockingCollection<string> _deleteBuffer = new BlockingCollection<string>();
        private int _maxEmptyBufferLength = 0;

        WaveInEvent waveIn;
        private WaveFileWriter _waveWriter;
        string tempFilename = "";

        Action<ResultCommandADO> actProcessResultCommand;
        DelegateGetCurrentCommandActionLink actGetCurrentCommandActionLink;
        DelegateGetCurrentCommandActionLink actGetCurrentModulelink;
        Action actProcessWhileTimeout;

        Mic2Wss objMic2Wss = null;


        public VoiceCommandProcessor(Action<ResultCommandADO> _actProcessResultCommand)
        {
            this.actProcessResultCommand = _actProcessResultCommand;
            this.objMic2Wss = new Mic2Wss();
            this.objMic2Wss.MicNumber = 0; // select Mic
        }

        public void SetDelegateTimeout(Action _actProcessWhileTimeout)
        {
            this.actProcessWhileTimeout = _actProcessWhileTimeout;
        }

        public void SetCommandActionLink(DelegateGetCurrentCommandActionLink _actGetCurrentCommandActionLink)
        {
            this.actGetCurrentCommandActionLink = _actGetCurrentCommandActionLink;
        }

        public void SetCommandModuleLink(DelegateGetCurrentCommandActionLink _actGetCurrentModulelink)
        {
            this.actGetCurrentModulelink = _actGetCurrentModulelink;
        }

        public void InitialDicAICommandActionControlConfig(List<CommandTypeADO> _listAICommandActionControlConfig)
        {
            this.listAICommandActionControlConfig = _listAICommandActionControlConfig;
        }

        public List<ResultCommandADO> GetResultData()
        {
            return _apiResultBuffer.ToList();
        }

        public List<CommandTypeADO> GetAICommandActionControlConfigData()
        {
            return listAICommandActionControlConfig;
        }

        /// <summary>
        /// Runs the main loop until "exit" or "quit" is heard.
        /// </summary>
        public async Task RunAsync()
        {
            Inventec.Common.Logging.LogSystem.Debug("RunAsync. 1");
            objMic2Wss.WssUrl = String.Format("{0}?sample_rate={1}&API_KEY={2}&version={3}", CommandCFG.Vais__WssUrl, CommandCFG.SampleRate, CommandCFG.Vais__APIKEY, CommandCFG.Vais__version);//"wss://nhaplieu.vais.vn?sample_rate=16000&API_KEY=14a37fbc-382e-11ea-bc1d-0242ac140007&max_alternate=1"
            objMic2Wss.SampleRate = CommandCFG.Vais__SampleRate;
            objMic2Wss.BufferMillisecond = CommandCFG.Vais__BufferMillisecond;
            //Cơ bản
            //Y tế
            //Y tế Isofh
            //Ngày tháng
            //Ký tự
            if (!String.IsNullOrEmpty(CommandCFG.Vais__ModelName))
            {
                objMic2Wss.ModelName = CommandCFG.Vais__ModelName;
            }
            Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => objMic2Wss.WssUrl), objMic2Wss.WssUrl));
            await objMic2Wss.StartListening();
            Inventec.Common.Logging.LogSystem.Debug("RunAsync. 1.1");
            while (objMic2Wss.IsListening())
            {
                Inventec.Common.Logging.LogSystem.Debug("RunAsync. 1.2");
                VaisAsrResult result = await objMic2Wss.Transcribe();

                dynamic stuff = JsonConvert.DeserializeObject(result.Text);

                if (stuff != null)
                {
                    if (stuff["text"] != null)
                    {
                        result.Transcript = stuff["text"].ToString();
                        if (stuff["id"] == null) { result.Code = "000"; } else { result.Code = stuff["id"].ToString(); }
                        if (stuff["status"] == null) { result.Status = 1; } else { Int32.Parse(stuff["status"].ToString()); }
                        if (stuff["type"] == null) { result.Type = ""; } else { result.Type = stuff["type"].ToString(); }
                        if (stuff["isFinal"] != null)
                        {
                            bool isFinal = Boolean.Parse(stuff["isFinal"].ToString());
                            result.IsFinal = isFinal;
                        }
                    }
                    else
                    {
                        result.Transcript = "";
                        result.IsFinal = true;
                    }
                }
                else
                {
                    result.Transcript = "";
                    result.IsFinal = true;
                }
                Inventec.Common.Logging.LogSystem.Debug("objMic2Wss.Transcribe____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => result), result));
                if (result.IsFinal)
                {
                    ResultCommandADO resultCommandADO = new ResultCommandADO();
                    resultCommandADO.text = result.Transcript;
                    resultCommandADO.success = true;
                    CallBackProcessResultCommand(resultCommandADO);
                    //txtTranscript.AppendText(txtChunkText.Text + System.Environment.NewLine);
                    //txtChunkText.Text = "";
                    try
                    {
                        await objMic2Wss.ClosingMessage();
                        await objMic2Wss.ReOpeningMessage();
                    }
                    catch (Exception exx)
                    {
                        Inventec.Common.Logging.LogSystem.Warn(exx);
                    }
                }
            }
            Inventec.Common.Logging.LogSystem.Debug("RunAsync. 2");
        }

        public void UnitTestReadVoice()
        {
            string tempFolder = CommandUtils.GenerateTempFolderWithin();

            string[] fileEntries = Directory.EnumerateFiles(tempFolder, "*.wav", SearchOption.TopDirectoryOnly)
                .Where(s => (s.EndsWith(".wav") || s.EndsWith(".mp3") || s.EndsWith(".mp4"))).ToArray();

            Inventec.Common.Logging.LogSystem.Info(" fileEntries: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => fileEntries), fileEntries));
            Inventec.Common.Logging.LogSystem.Info("apiTypeAI: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => CommandCFG.apiTypeAI), CommandCFG.apiTypeAI));

            if (fileEntries != null && fileEntries.Count() > 0)
            {
                foreach (var item in fileEntries)
                {
                    if (CommandCFG.apiTypeAI == CommandCFG.apiTypeAI__WITAI)
                    {
                        ResultCommandADO result = new ProcessSpeechWit().Run(File.ReadAllBytes(item)).Result;
                        if (result != null && !String.IsNullOrEmpty(result.text))
                        {
                            CallBackProcessResultCommand(result);
                            _apiResultBuffer.Add(result);
                        }
                    }
                    else if (CommandCFG.apiTypeAI == CommandCFG.apiTypeAI__RIKKEIAI)
                    {
                        ResultCommandADO result = new ProcessSpeechRikkie().Run(File.ReadAllBytes(item)).Result;
                        if (result != null && result.status == 200)
                        {
                            result.text = result.message;
                            result.text = result.text.Replace("\n", "");
                            CallBackProcessResultCommand(result);
                            _apiResultBuffer.Add(result);
                        }
                    }
                    else if (CommandCFG.apiTypeAI == CommandCFG.apiTypeAI__VAIS)
                    {
                        ResultCommandADO result = new ProcessSpeechVais().Run(File.ReadAllBytes(item)).Result;
                        if (result != null && result.status == 200)
                        {
                            result.text = result.message;
                            result.text = result.text.Replace("\n", "");
                            CallBackProcessResultCommand(result);
                            _apiResultBuffer.Add(result);
                        }
                    }
                }

                var _apiResultBuffer1 = _apiResultBuffer.Where(o => !String.IsNullOrEmpty(o.text) || !String.IsNullOrEmpty(o._text)).ToList();
                if (_apiResultBuffer1 != null && _apiResultBuffer1.Count() > 0)
                {
                }
            }
        }

        /// <summary>fd
        /// Takes a single sample chunk from the microphone buffer, keeps a local copy
        /// (in case we need to send it again in a new request) and sends it to the server.
        /// </summary>
        /// <returns></returns>
        private async Task TransferMicrophoneChunkAsync()
        {
            if (_microphoneBuffer.Count > 0)
            {
                // This will block - but only for ~100ms, unless something's really broken.
                var chunk = _microphoneBuffer.Take();
                _processingBuffer.AddLast(chunk);
                //await WriteAudioChunk(chunk);
            }
            Inventec.Common.Logging.LogSystem.Info("TransferMicrophoneChunkAsync._microphoneBuffer.Count=" + (_microphoneBuffer.Count));
        }

        /// <summary>
        /// Processes responses received so far from the server,
        /// returning whether "exit" or "quit" have been heard.
        /// </summary>
        private bool ProcessResponses()
        {
            try
            {
                if (_deleteBuffer != null && _deleteBuffer.Count > 0)
                {
                    foreach (var item in _deleteBuffer)
                    {
                        try
                        {
                            File.Delete(item);
                        }
                        catch { }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            if (_apiResultBuffer != null && _apiResultBuffer.Count > 0 && _apiResultBuffer.Any(o => o.text != null && (o.text.Contains("exit") || o.text.Contains("quit"))))
            //if (transcript.ToLowerInvariant().Contains("exit") ||
            //    transcript.ToLowerInvariant().Contains("quit"))
            {
                Inventec.Common.Logging.LogSystem.Info("ProcessResponses=> Transcript exit");
                if (waveIn != null)
                    waveIn.StopRecording();
                return false;
            }

            // Rather than explicitly iterate over the list, we just always deal with the first
            // element, either removing it or stopping.            
            Inventec.Common.Logging.LogSystem.Info("ProcessResponses: _processingBuffer.Count=" + (_processingBuffer != null ? _processingBuffer.Count : 0) + "__");
            if (_processingBuffer != null && _processingBuffer.Count > 0 && _processingBuffer.Any(o => o.IsComplate))
                while (_processingBuffer.Any(o => o.IsComplate))
                {
                    var removeList = _processingBuffer.Where(t => t.IsComplate).ToList();
                    Inventec.Common.Logging.LogSystem.Info("ProcessResponses: removeList.Count=" + (removeList != null ? removeList.Count : 0) + "__");
                    foreach (var item in removeList)
                    {
                        _processingBuffer.Remove(item);
                    }
                }

            return true;
        }

        /// <summary>
        /// Starts a new RPC streaming call if necessary. This will be if either it's the first call
        /// (so we don't have a current request) or if the current request will time out soon.
        /// In the latter case, after starting the new request, we copy any chunks we'd already sent
        /// in the previous request which hadn't been included in a "final result".
        /// </summary>
        private async Task MaybeStartStreamAsync()
        {
            _processingBufferStart = DateTime.Now;
            if (_processingBuffer != null && _processingBuffer.Any(o => !o.IsProcessing && !o.IsComplate))
            {
                var noneProcesss = _processingBuffer.Where(o => !o.IsProcessing && !o.IsComplate).ToList();
                Inventec.Common.Logging.LogSystem.Info("Writing " + (noneProcesss != null ? noneProcesss.Count : 0) + " chunks into the new stream.");
                if (noneProcesss != null && noneProcesss.Count > 0)
                    foreach (var chunk in noneProcesss)
                    {
                        await WriteAudioChunk(chunk);
                    }

                //if ((_rpcStreamDeadline == null || _rpcStreamDeadline == DateTime.MinValue || (_rpcStreamDeadline != null && _rpcStreamDeadline != DateTime.MinValue && _processingBufferStart.Second - _rpcStreamDeadline.Second > 4)) && !String.IsNullOrEmpty(tempFilename) && File.Exists(tempFilename))
                //{
                //    Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => _rpcStreamDeadline), _rpcStreamDeadline)
                //        + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => _processingBufferStart), _processingBufferStart)
                //        + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => tempFilename), tempFilename));
                //    ResultCommandADO result = await new ProcessSpeechRikkie().Run(File.ReadAllBytes(tempFilename));
                //    if (result != null)
                //        _apiResultBuffer.Add(result);
                //    _rpcStreamDeadline = DateTime.Now;
                //}
            }
        }

        /// <summary>
        /// Writes a single chunk to the RPC stream.
        /// </summary>
        private async Task WriteAudioChunk(ProcessCommandADO processCommandADO)
        {
            Inventec.Common.Logging.LogSystem.Info("WriteAudioChunk=> chunk.Length=" + (processCommandADO != null ? processCommandADO.BytesRecorded : 0));
            if (processCommandADO.Buffer != null && !processCommandADO.IsProcessing)
            {
                processCommandADO.IsProcessing = true;



                var resultCommandADO = await new ProcessSpeechVais().Run(processCommandADO.Buffer);
                if (resultCommandADO != null && resultCommandADO.status == 2 && resultCommandADO.message != "\n")
                {
                    _apiResultBuffer.Add(resultCommandADO);
                    processCommandADO.IsComplate = true;
                    CallBackProcessResultCommand(resultCommandADO);
                }
                Inventec.Common.Logging.LogSystem.Debug("WriteAudioChunk: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => resultCommandADO), resultCommandADO));

                //string tempFilename1 = CommandUtils.GenerateTempFileWithin();
                //WaveFileWriter _waveWriter1 = new WaveFileWriter(tempFilename1, waveIn.WaveFormat);
                //await _waveWriter1.WriteAsync(processCommandADO.Buffer, 0, processCommandADO.BytesRecorded);
                //_waveWriter1.Flush();
                //_waveWriter1 = null;

                //if (CommandCFG.apiTypeAI == CommandCFG.apiTypeAI__RIKKEIAI)
                //{
                //    var resultCommandADO = await new ProcessSpeechRikkie().Run(processCommandADO.Buffer);
                //    if (resultCommandADO != null && resultCommandADO.status == 200 && resultCommandADO.message != "\n")
                //    {
                //        resultCommandADO.text = resultCommandADO.message;
                //        resultCommandADO.text = resultCommandADO.text.Replace("\n", "");
                //        resultCommandADO.success = true;
                //        _apiResultBuffer.Add(resultCommandADO);
                //        processCommandADO.IsComplate = true;

                //        CallBackProcessResultCommand(resultCommandADO);
                //    }
                //    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => resultCommandADO), resultCommandADO));
                //}
                //else if (CommandCFG.apiTypeAI == CommandCFG.apiTypeAI__WITAI)
                //{
                //    var resultCommandADO = await new ProcessSpeechWit().Run(processCommandADO.Buffer);

                //    if (resultCommandADO != null && !String.IsNullOrEmpty(resultCommandADO.text) && resultCommandADO.text != "\n")
                //    {
                //        resultCommandADO.success = true;
                //        _apiResultBuffer.Add(resultCommandADO);
                //        processCommandADO.IsComplate = true;

                //        CallBackProcessResultCommand(resultCommandADO);
                //    }
                //    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => resultCommandADO), resultCommandADO));
                //}
                //else if (CommandCFG.apiTypeAI == CommandCFG.apiTypeAI__VAIS)
                //{
                //    //var resultCommandADO = await new ProcessSpeechVais().Run(processCommandADO.Buffer);
                //    //if (resultCommandADO != null && resultCommandADO.status == 200 && resultCommandADO.message != "\n")
                //    //{
                //    //resultCommandADO.text = resultCommandADO.message;
                //    //resultCommandADO.text = resultCommandADO.text.Replace("\n", "");
                //    //resultCommandADO.success = true;

                //    //_apiResultBuffer.Add(resultCommandADO);
                //    //processCommandADO.IsComplate = true;

                //    //CallBackProcessResultCommand(resultCommandADO);
                //    //}
                //    var resultCommandADO = await new ProcessSpeechVais().Run(processCommandADO.Buffer);
                //    if (resultCommandADO != null && resultCommandADO.status == 2 && resultCommandADO.message != "\n")
                //    {
                //        //resultCommandADO.text = resultCommandADO.message;
                //        //resultCommandADO.text = resultCommandADO.text.Replace("\n", "");
                //        //resultCommandADO.success = true;

                //        _apiResultBuffer.Add(resultCommandADO);
                //        processCommandADO.IsComplate = true;

                //        CallBackProcessResultCommand(resultCommandADO);
                //    }
                //    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => resultCommandADO), resultCommandADO));
                //}
                Inventec.Common.Logging.LogSystem.Info("WriteAudioChunk=> end");
            }
        }

        void CallBackProcessResultCommand(ResultCommandADO resultCommandADO)
        {
            try
            {
                CommandTypeADO commandType = null;
                string currentCommandActionLink = actGetCurrentCommandActionLink != null ? actGetCurrentCommandActionLink() : "";
                string currentModulelink = actGetCurrentModulelink != null ? actGetCurrentModulelink() : "";
                var listAICommandActionControlConfigByModule = (listAICommandActionControlConfig != null && listAICommandActionControlConfig.Count > 0) ? listAICommandActionControlConfig.Where(o => (String.IsNullOrEmpty(o.ModuleLink) || (!String.IsNullOrEmpty(o.ModuleLink) && o.ModuleLink == currentModulelink))).ToList() : null;
                if (listAICommandActionControlConfigByModule != null && listAICommandActionControlConfigByModule.Count > 0)
                {
                    foreach (var item in listAICommandActionControlConfigByModule)
                    {
                        commandType = (item.listText != null && item.listText.Count > 0 && item.listText.Exists(o => o == resultCommandADO.text) &&
                            String.IsNullOrEmpty(item.ModuleLink) &&
                            item.commandType == IMSys.DbConfig.VVA_RS.VVA_COMMAND_TYPE.COMMAND_TYPE__MENU) ? item : null;

                        if (commandType == null && !String.IsNullOrEmpty(currentCommandActionLink))
                        {
                            commandType = (
                                (item.commandActionLink == currentCommandActionLink) &&
                                item.commandType == IMSys.DbConfig.VVA_RS.VVA_COMMAND_TYPE.COMMAND_TYPE__CUSTOM) ? item : null;
                        }

                        if (commandType == null)
                        {
                            commandType = (item.listText != null && item.listText.Count > 0 && item.listText.Exists(o => o == resultCommandADO.text) &&
                            ((item.commandType == IMSys.DbConfig.VVA_RS.VVA_COMMAND_TYPE.COMMAND_TYPE__CLEAR
                                || item.commandType == IMSys.DbConfig.VVA_RS.VVA_COMMAND_TYPE.COMMAND_TYPE__ENTER
                                || item.commandType == IMSys.DbConfig.VVA_RS.VVA_COMMAND_TYPE.COMMAND_TYPE__FOCUS)
                                || (!String.IsNullOrEmpty(currentCommandActionLink) && item.commandActionLink == currentCommandActionLink)) &&
                            (item.commandType == IMSys.DbConfig.VVA_RS.VVA_COMMAND_TYPE.COMMAND_TYPE__FOCUS
                                || item.commandType == IMSys.DbConfig.VVA_RS.VVA_COMMAND_TYPE.COMMAND_TYPE__CLEAR
                                || item.commandType == IMSys.DbConfig.VVA_RS.VVA_COMMAND_TYPE.COMMAND_TYPE__CLICK
                                || item.commandType == IMSys.DbConfig.VVA_RS.VVA_COMMAND_TYPE.COMMAND_TYPE__ENTER)) ? item : null;
                        }

                        if (commandType != null)
                        {
                            resultCommandADO.commandType = commandType.commandType;
                            resultCommandADO.commandActionLink = commandType.commandActionLink;
                            Inventec.Common.Logging.LogSystem.Info("CallBackProcessResultCommand==> tim thay cau hinh command action control thoa man du lieu voice:" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => resultCommandADO), resultCommandADO) + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => item), item));
                            break;
                        }
                    }

                    if (commandType == null && !String.IsNullOrEmpty(currentCommandActionLink))
                    {
                        commandType = new CommandTypeADO();
                        commandType.commandActionLink = currentCommandActionLink;
                        commandType.commandType = (int)IMSys.DbConfig.VVA_RS.VVA_COMMAND_TYPE.COMMAND_TYPE__INPUT;
                        Inventec.Common.Logging.LogSystem.Info("CallBackProcessResultCommand==> khong tim thay cau hinh command action control thoa man du lieu voice, kiem tra co control nhap lieu dang focus => tu dong dien text sau khi qua nhan dien giong noi vao o nhap lieu do____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => commandType), commandType) + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentCommandActionLink), currentCommandActionLink));
                    }

                    if (commandType != null)
                    {
                        resultCommandADO.commandType = commandType.commandType;
                        resultCommandADO.commandActionLink = commandType.commandActionLink;
                    }

                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentCommandActionLink), currentCommandActionLink)
                                + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => commandType), commandType));

                    if (this.actProcessResultCommand != null)
                        this.actProcessResultCommand(resultCommandADO);
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Debug("Khong co danh sach cau hinh dung chung hoac theo modulelink___Tat ca cau hinh____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => listAICommandActionControlConfig), listAICommandActionControlConfig));
                }
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => listAICommandActionControlConfigByModule), listAICommandActionControlConfigByModule)
                    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => resultCommandADO), resultCommandADO)
                    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentModulelink), currentModulelink)
                    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentCommandActionLink), currentCommandActionLink));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Starts listening to input device 0, and adds an event handler which simply adds
        /// the sample to the microphone buffer. The returned <see cref="WaveInEvent"/> must
        /// be disposed after we've finished with it.
        /// </summary>
        public WaveInEvent StartListening()
        {
            return null;
            //CommandUtils.ResetVoiceCacheWithinTempFolder();

            //waveIn = new WaveInEvent
            //{
            //    DeviceNumber = 0,
            //    WaveFormat = new WaveFormat(CommandCFG.SampleRate, CommandCFG.ChannelCount),
            //    BufferMilliseconds = CommandCFG.BufferMilliseconds,//300ms
            //};

            //_maxEmptyBufferLength = 0;
            //waveIn.DataAvailable += OnDataAvailable;

            ////tempFilename = CommandUtils.GenerateTempFileWithin();
            ////_waveWriter = new WaveFileWriter(tempFilename, waveIn.WaveFormat);

            //waveIn.StartRecording();

            //return waveIn;
        }

        float CalulatedMax(byte[] buffer, int bytesRecorded)
        {
            float max = 0;
            // interpret as 16 bit audio
            for (int index = 0; index < bytesRecorded; index += 2)
            {
                short sample = (short)((buffer[index + 1] << 8) |
                                        buffer[index + 0]);
                // to floating point
                var sample32 = sample / 32768f;
                // absolute value 
                if (sample32 < 0) sample32 = -sample32;
                // is this the max value?
                if (sample32 > max) max = sample32;
            }
            return max;
        }

        private void OnDataAvailable(object sender, WaveInEventArgs args)
        {
            float max = CalulatedMax(args.Buffer, args.BytesRecorded);
            Inventec.Common.Logging.LogSystem.Info("max=" + max);//we calulated max as a floating point value between 0.0f and 1.0f,         
            ProcessCommandADO ProcessCommandADO = new ProcessCommandADO();
            ProcessCommandADO.Buffer = args.Buffer;
            ProcessCommandADO.BytesRecorded = args.BytesRecorded;
            _microphoneBuffer.Add(ProcessCommandADO);
            Inventec.Common.Logging.LogSystem.Info("waveIn.DataAvailable:_microphoneBuffer.Count=" + _microphoneBuffer.Count);


            //if (max >= 0.2f)
            //{
            //    _waveWriter.Write(args.Buffer, 0, args.BytesRecorded);
            //    _maxEmptyBuffer = new BlockingCollection<float>();
            //    _maxEmptyBufferLength = 0;
            //    Inventec.Common.Logging.LogSystem.Info("waveIn.DataAvailable:clear maxEmptyBuffer => max=" + max + "____maxEmptyBuffer.Count=" + _maxEmptyBuffer.Count);
            //}
            //else
            //{
            //    _maxEmptyBuffer.Add(max);
            //    _maxEmptyBufferLength += 1;
            //    Inventec.Common.Logging.LogSystem.Info("waveIn.DataAvailable:add to _maxEmptyBuffer => _maxEmptyBuffer.Count=" + _maxEmptyBuffer.Count + ", _maxEmptyBufferLength=" + _maxEmptyBufferLength);
            //}

            //if (_maxEmptyBufferLength >= 100)
            //{
            //    if (this.actProcessWhileTimeout != null)
            //        this.actProcessWhileTimeout();
            //    return;
            //}

            //if (_maxEmptyBuffer != null && _maxEmptyBuffer.Count >= 4)
            //{
            //    string tempFilenameBreak = CommandUtils.GenerateTempFileWithin();

            //    File.Copy(tempFilename, tempFilenameBreak, true);

            //    _deleteBuffer.Add(tempFilename);

            //    _waveWriter.Flush();
            //    _waveWriter = null;
            //    var arrVoice = File.ReadAllBytes(tempFilenameBreak);
            //    float maxSum = CalulatedMax(arrVoice, arrVoice.Length);
            //    Inventec.Common.Logging.LogSystem.Debug(
            //        Inventec.Common.Logging.LogUtil.TraceData("tempFilenameBreak", tempFilenameBreak)
            //        + Inventec.Common.Logging.LogUtil.TraceData("arrVoice.Length", arrVoice.Length)
            //        + Inventec.Common.Logging.LogUtil.TraceData("maxSum", maxSum));
            //    if (maxSum >= 0.1f)
            //    {
            //        ProcessCommandADO ProcessCommandADO = new ProcessCommandADO();
            //        ProcessCommandADO.Buffer = arrVoice;
            //        ProcessCommandADO.BytesRecorded = arrVoice.Length;
            //        _microphoneBuffer.Add(ProcessCommandADO);
            //        Inventec.Common.Logging.LogSystem.Info("waveIn.DataAvailable:_microphoneBuffer.Count=" + _microphoneBuffer.Count);
            //    }
            //    tempFilename = CommandUtils.GenerateTempFileWithin();
            //    _waveWriter = new WaveFileWriter(tempFilename, waveIn.WaveFormat);
            //    _deleteBuffer.Add(tempFilenameBreak);
            //    _maxEmptyBuffer = new BlockingCollection<float>();
            //}
        }

        public void Stop()
        {
            try
            {
                this.objMic2Wss.StopListening();
                //if (waveIn != null)
                //    waveIn.StopRecording();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
