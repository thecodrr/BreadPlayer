using Microsoft.Graphics.Canvas.Effects;
using System.Numerics;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;

namespace BreadPlayer.Effects
{
    public class BackDrop : Control
    {
        private Compositor _mCompositor;
        private SpriteVisual _mBlurVisual;
        private CompositionBrush _mBlurBrush;
        private Visual _mRootVisual;

#if SDKVERSION_14393
        private bool _mSetUpExpressions;
#endif

        public BackDrop()
        {
            _mRootVisual = ElementCompositionPreview.GetElementVisual(this);

            Compositor = _mRootVisual.Compositor;

            _mBlurVisual = Compositor.CreateSpriteVisual();

#if SDKVERSION_14393

            CompositionEffectBrush brush = BuildBlurBrush();
            brush.SetSourceParameter("source", _mCompositor.CreateBackdropBrush());
            _mBlurBrush = brush;
            _mBlurVisual.Brush = _mBlurBrush;

            BlurAmount = 9;
#else
            m_blurBrush = Compositor.CreateColorBrush(Colors.White);
            m_blurVisual.Brush = m_blurBrush;
#endif
            ElementCompositionPreview.SetElementChildVisual(this, _mBlurVisual);

            Loading += OnLoading;
            Unloaded += OnUnloaded;
        }

        public const string BlurAmountProperty = nameof(BlurAmount);

        public double BlurAmount
        {
            get
            {
                float value = 0;
#if SDKVERSION_14393
                _mRootVisual.Properties.TryGetScalar(BlurAmountProperty, out value);
#endif
                return value;
            }
            set
            {
#if SDKVERSION_14393
                if (!_mSetUpExpressions)
                {
                    _mBlurBrush.Properties.InsertScalar("Blur.BlurAmount", (float)value);
                }
                _mRootVisual.Properties.InsertScalar(BlurAmountProperty, (float)value);
#endif
            }
        }

        public Compositor Compositor
        {
            get => _mCompositor;

            private set => _mCompositor = value;
        }

#pragma warning disable 1998

        private async void OnLoading(FrameworkElement sender, object args)
        {
            SizeChanged += OnSizeChanged;
            OnSizeChanged(this, null);
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            SizeChanged -= OnSizeChanged;
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (_mBlurVisual != null)
            {
                _mBlurVisual.Size = new Vector2((float)ActualWidth, (float)ActualHeight);
            }
        }

#if SDKVERSION_14393

        private void SetUpPropertySetExpressions()
        {
            _mSetUpExpressions = true;

            var exprAnimation = Compositor.CreateExpressionAnimation();
            exprAnimation.Expression = $"sourceProperties.{BlurAmountProperty}";
            exprAnimation.SetReferenceParameter("sourceProperties", _mRootVisual.Properties);

            _mBlurBrush.Properties.StartAnimation("Blur.BlurAmount", exprAnimation);
        }

        private CompositionEffectBrush BuildBlurBrush()
        {
            GaussianBlurEffect blurEffect = new GaussianBlurEffect
            {
                Name = "Blur",
                BlurAmount = 0.0f,
                BorderMode = EffectBorderMode.Hard,
                Optimization = EffectOptimization.Balanced,
                Source = new CompositionEffectSourceParameter("source")
            };

            var factory = Compositor.CreateEffectFactory(
                blurEffect,
                new[] { "Blur.BlurAmount" }
                );

            CompositionEffectBrush brush = factory.CreateBrush();
            return brush;
        }

        public CompositionPropertySet VisualProperties
        {
            get
            {
                if (!_mSetUpExpressions)
                {
                    SetUpPropertySetExpressions();
                }
                return _mRootVisual.Properties;
            }
        }

#endif
    }
}