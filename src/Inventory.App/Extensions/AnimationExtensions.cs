﻿#region copyright
// ******************************************************************
// Copyright (c) Microsoft. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THE CODE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH
// THE CODE OR THE USE OR OTHER DEALINGS IN THE CODE.
// ******************************************************************
#endregion

using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Hosting;

namespace Inventory.Animations
{
    public static class AnimationExtensions
    {
        public static void Fade(this UIElement element, double milliseconds, double start, double end, CompositionEasingFunction easingFunction = null)
        {
            element.StartAnimation(nameof(Visual.Opacity), CreateScalarAnimation(milliseconds, start, end, easingFunction));
        }

        public static void TranslateX(this UIElement element, double milliseconds, double start, double end, CompositionEasingFunction easingFunction = null)
        {
            ElementCompositionPreview.SetIsTranslationEnabled(element, true);
            element.StartAnimation("Translation.X", CreateScalarAnimation(milliseconds, start, end, easingFunction));
        }

        public static void TranslateY(this UIElement element, double milliseconds, double start, double end, CompositionEasingFunction easingFunction = null)
        {
            ElementCompositionPreview.SetIsTranslationEnabled(element, true);
            element.StartAnimation("Translation.Y", CreateScalarAnimation(milliseconds, start, end, easingFunction));
        }

        public static void Scale(this FrameworkElement element, double milliseconds, double start, double end, CompositionEasingFunction easingFunction = null)
        {
            element.SetCenterPoint(element.ActualWidth / 2.0, element.ActualHeight / 2.0);
            var vectorStart = new Vector3((float)start, (float)start, 0);
            var vectorEnd = new Vector3((float)end, (float)end, 0);
            element.StartAnimation(nameof(Visual.Scale), CreateVector3Animation(milliseconds, vectorStart, vectorEnd, easingFunction));
        }

        public static void Blur(this UIElement element, double amount)
        {
            CompositionEffectBrush brush = CreateBlurEffectBrush(amount);
            element.SetBrush(brush);
        }
        public static void Blur(this UIElement element, double milliseconds, double start, double end, CompositionEasingFunction easingFunction = null)
        {
            CompositionEffectBrush brush = CreateBlurEffectBrush();
            element.SetBrush(brush);
            brush.StartAnimation("Blur.BlurAmount", CreateScalarAnimation(milliseconds, start, end, easingFunction));
        }

        public static void Grayscale(this UIElement element)
        {
            CompositionEffectBrush brush = CreateGrayscaleEffectBrush();
            element.SetBrush(brush);
        }

        public static void SetBrush(this UIElement element, CompositionBrush brush)
        {
            SpriteVisual spriteVisual = CreateSpriteVisual(element);
            spriteVisual.Brush = brush;
            ElementCompositionPreview.SetElementChildVisual(element, spriteVisual);
        }

        public static void ClearEffects(this UIElement element)
        {
            ElementCompositionPreview.SetElementChildVisual(element, null);
        }

        public static SpriteVisual CreateSpriteVisual(UIElement element)
        {
            return CreateSpriteVisual(ElementCompositionPreview.GetElementVisual(element));
        }
        public static SpriteVisual CreateSpriteVisual(Visual elementVisual)
        {
            Compositor compositor = elementVisual.Compositor;
            SpriteVisual spriteVisual = compositor.CreateSpriteVisual();
            ExpressionAnimation expression = compositor.CreateExpressionAnimation();
            expression.Expression = "visual.Size";
            expression.SetReferenceParameter("visual", elementVisual);
            spriteVisual.StartAnimation(nameof(Visual.Size), expression);
            return spriteVisual;
        }

        public static void SetCenterPoint(this UIElement element, double x, double y)
        {
            Visual visual = ElementCompositionPreview.GetElementVisual(element);
            visual.CenterPoint = new Vector3((float)x, (float)y, 0);
        }

        public static void StartAnimation(this UIElement element, string propertyName, CompositionAnimation animation)
        {
            Visual visual = ElementCompositionPreview.GetElementVisual(element);
            visual.StartAnimation(propertyName, animation);
        }

        public static CompositionAnimation CreateScalarAnimation(double milliseconds, double start, double end, CompositionEasingFunction easingFunction = null)
        {
            ScalarKeyFrameAnimation animation = Window.Current.Compositor.CreateScalarKeyFrameAnimation();
            animation.InsertKeyFrame(0.0f, (float)start, easingFunction);
            animation.InsertKeyFrame(1.0f, (float)end, easingFunction);
            animation.Duration = TimeSpan.FromMilliseconds(milliseconds);
            return animation;
        }

        public static CompositionAnimation CreateVector3Animation(double milliseconds, Vector3 start, Vector3 end, CompositionEasingFunction easingFunction = null)
        {
            Vector3KeyFrameAnimation animation = Window.Current.Compositor.CreateVector3KeyFrameAnimation();
            animation.InsertKeyFrame(0.0f, start);
            animation.InsertKeyFrame(1.0f, end);
            animation.Duration = TimeSpan.FromMilliseconds(milliseconds);
            return animation;
        }

        public static CompositionEffectBrush CreateBlurEffectBrush(double amount = 0.0)
        {
            var effect = new GaussianBlurEffect
            {
                Name = "Blur",
                BlurAmount = (float)amount,
                Source = new CompositionEffectSourceParameter("source")
            };

            Compositor compositor = Window.Current.Compositor;
            CompositionEffectFactory factory = compositor.CreateEffectFactory(effect, new[] { "Blur.BlurAmount" });
            CompositionEffectBrush brush = factory.CreateBrush();
            brush.SetSourceParameter("source", compositor.CreateBackdropBrush());
            return brush;
        }

        public static CompositionEffectBrush CreateGrayscaleEffectBrush()
        {
            var effect = new GrayscaleEffect
            {
                Name = "Grayscale",
                Source = new CompositionEffectSourceParameter("source")
            };

            Compositor compositor = Window.Current.Compositor;
            CompositionEffectFactory factory = compositor.CreateEffectFactory(effect);
            CompositionEffectBrush brush = factory.CreateBrush();
            brush.SetSourceParameter("source", compositor.CreateBackdropBrush());
            return brush;
        }
    }
}
