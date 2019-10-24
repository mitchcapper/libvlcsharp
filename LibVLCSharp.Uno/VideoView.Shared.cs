﻿using System;
using LibVLCSharp.Shared;
using Windows.ApplicationModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace LibVLCSharp.Platforms.UWP
{
    /// <summary>
    /// Video view
    /// </summary>
    public partial class VideoView : Control, IVideoView
    {
        /// <summary>
        /// Occurs when the <see cref="VideoView"/> is fully loaded
        /// </summary>
        public event EventHandler<InitializedEventArgs>? Initialized;

        /// <summary>
        /// Initializes a new instance of <see cref="VideoView"/> class
        /// </summary>
        public VideoView()
        {
            DefaultStyleKey = typeof(VideoView);
            Loaded += (sender, e) => Initialized?.Invoke(this, new InitializedEventArgs(SwapChainOptions));
        }

        private Border? Border { get; set; }

        /// <summary>
        /// Gets the swapchain parameters to pass to the <see cref="LibVLC"/> constructor.
        /// Calling this property will throw an <see cref="InvalidOperationException"/> if the VideoView is not yet fully loaded.
        /// </summary>
        /// <returns>The list of arguments to be given to the <see cref="LibVLC"/> constructor</returns>
        public string[] SwapChainOptions
        {
            get
            {
                if (!IsLoaded)
                {
                    throw new InvalidOperationException("You must wait for the VideoView to be loaded before calling GetSwapChainOptions()");
                }

                return Array.Empty<string>();
            }
        }

        /// <summary>
        /// Identifies the <see cref="MediaPlayer"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MediaPlayerProperty = DependencyProperty.Register(nameof(MediaPlayer),
            typeof(Shared.MediaPlayer), typeof(VideoView), new PropertyMetadata(new PropertyChangedCallback(OnMediaPlayerPropertyChanged)));
        /// <summary>
        /// Gets the <see cref="Shared.MediaPlayer"/> instance.
        /// </summary>
        public Shared.MediaPlayer MediaPlayer
        {
            get => (Shared.MediaPlayer)GetValue(MediaPlayerProperty);
            set => SetValue(MediaPlayerProperty, value);
        }

        private static void OnMediaPlayerPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            ((VideoView)dependencyObject).UpdateUnderlyingVideoView();
        }

        /// <summary>
        /// Invoked whenever application code or internal processes (such as a rebuilding layout pass) call <see cref="Control.ApplyTemplate"/>.
        /// In simplest terms, this means the method is called just before a UI element displays in your app
        /// </summary>
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (!DesignMode.DesignModeEnabled)
            {
                Border = (Border)GetTemplateChild("Border");
                UpdateUnderlyingVideoView();
            }
        }

        private void UpdateUnderlyingVideoView()
        {
            var underlyingVideoView = UnderlyingVideoView;
            var mediaPlayer = MediaPlayer;
            if (mediaPlayer == null)
            {
                UnderlyingVideoView = null;
                if (underlyingVideoView != null)
                {
                    underlyingVideoView.MediaPlayer = null;
                    underlyingVideoView.Dispose();
                }
            }
            else
            {
                if (Border != null)
                {
                    if (underlyingVideoView == null)
                    {
                        underlyingVideoView = CreateVideoView();
                        UnderlyingVideoView = underlyingVideoView;
                        Border.Child = underlyingVideoView;
                    }
                    underlyingVideoView.MediaPlayer = mediaPlayer;
                }
            }
        }
    }
}
