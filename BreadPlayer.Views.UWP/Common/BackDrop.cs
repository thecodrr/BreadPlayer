using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;
using Microsoft.Graphics.Canvas.Effects;
using System;

namespace BreadPlayer.Effects
{
    public class BackDrop : Control
    {
        Compositor m_compositor;
        SpriteVisual m_blurVisual;
        CompositionBrush m_blurBrush;
        Visual m_rootVisual;

#if SDKVERSION_14393
        bool m_setUpExpressions;
#endif

        public BackDrop()
        { 
            m_rootVisual = ElementCompositionPreview.GetElementVisual(this as UIElement);

            Compositor = m_rootVisual.Compositor;

            m_blurVisual = Compositor.CreateSpriteVisual();

#if SDKVERSION_14393

            CompositionEffectBrush brush = BuildBlurBrush();
            brush.SetSourceParameter("source", m_compositor.CreateBackdropBrush());
            m_blurBrush = brush;
            m_blurVisual.Brush = m_blurBrush;

            BlurAmount = 9;
#else
            m_blurBrush = Compositor.CreateColorBrush(Colors.White);
            m_blurVisual.Brush = m_blurBrush;
#endif
            ElementCompositionPreview.SetElementChildVisual(this as UIElement, m_blurVisual);

            this.Loading += OnLoading;
            this.Unloaded += OnUnloaded;
        }

        public const string BlurAmountProperty = nameof(BlurAmount);

        public double BlurAmount
        {
            get
            {
                float value = 0;
#if SDKVERSION_14393
                m_rootVisual.Properties.TryGetScalar(BlurAmountProperty, out value);
#endif
                return value;
            }
            set
            {
#if SDKVERSION_14393
                if (!m_setUpExpressions)
                {
                    m_blurBrush.Properties.InsertScalar("Blur.BlurAmount", (float)value);
                }
                m_rootVisual.Properties.InsertScalar(BlurAmountProperty, (float)value);
#endif
            }
        }       

        public Compositor Compositor
        {
            get
            {
                return m_compositor;
            }

            private set
            {
                m_compositor = value;
            }
        }

#pragma warning disable 1998
        private async void OnLoading(FrameworkElement sender, object args)
        {
            this.SizeChanged += OnSizeChanged;
            OnSizeChanged(this, null);
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            this.SizeChanged -= OnSizeChanged;
        }


        private void OnSizeChanged(object sender, Windows.UI.Xaml.SizeChangedEventArgs e)
        {
            if (m_blurVisual != null)
            {
                m_blurVisual.Size = new System.Numerics.Vector2((float)this.ActualWidth, (float)this.ActualHeight);
            }
        }

#if SDKVERSION_14393
        private void SetUpPropertySetExpressions()
        {
            m_setUpExpressions = true;

            var exprAnimation = Compositor.CreateExpressionAnimation();
            exprAnimation.Expression = $"sourceProperties.{BlurAmountProperty}";
            exprAnimation.SetReferenceParameter("sourceProperties", m_rootVisual.Properties);

            m_blurBrush.Properties.StartAnimation("Blur.BlurAmount", exprAnimation);
        }


        private CompositionEffectBrush BuildBlurBrush()
        {
            GaussianBlurEffect blurEffect = new GaussianBlurEffect()
            {
                Name = "Blur",
                BlurAmount = 0.0f,
                BorderMode = EffectBorderMode.Hard,
                Optimization = EffectOptimization.Balanced,
                Source = new CompositionEffectSourceParameter("source"),
            };    

            var factory = Compositor.CreateEffectFactory(
                blurEffect,
                new[] { "Blur.BlurAmount"}
                );

            CompositionEffectBrush brush = factory.CreateBrush();
            return brush;
        }

        public CompositionPropertySet VisualProperties
        {
            get
            {
                if (!m_setUpExpressions)
                {
                    SetUpPropertySetExpressions();
                }
                return m_rootVisual.Properties;
            }
        }

#endif

    }
}
