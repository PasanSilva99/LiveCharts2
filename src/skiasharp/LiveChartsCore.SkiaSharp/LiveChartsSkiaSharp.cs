﻿// The MIT License(MIT)

// Copyright(c) 2021 Alberto Rodriguez Orozco & LiveCharts Contributors

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using LiveChartsCore.Kernel;
using LiveChartsCore.SkiaSharpView.Drawing;
using System;

namespace LiveChartsCore.SkiaSharpView
{
    /// <summary>
    /// Defines the default LiveCharts-SkiaSharp settings
    /// </summary>
    public static class LiveChartsSkiaSharp
    {
        private static readonly DefaultPaintTask<SkiaSharpDrawingContext> defaultPaintTask = new DefaultPaintTask<SkiaSharpDrawingContext>();

        /// <summary>
        /// Gets the default paint task.
        /// </summary>
        /// <value>
        /// The default paint.
        /// </value>
        public static DefaultPaintTask<SkiaSharpDrawingContext> DefaultPaint => defaultPaintTask;

        /// <summary>
        /// Gets the default platform builder.
        /// </summary>
        /// <value>
        /// The default platform builder.
        /// </value>
        public static Action<LiveChartsSettings> DefaultPlatformBuilder =>
            (LiveChartsSettings settings) => settings
                .AddDefaultMappers()
                .AddSkiaSharp();

        /// <summary>
        /// Adds SkiaSharp as the UI provider for LiveCharts.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <param name="builder">The builder.</param>
        /// <returns></returns>
        public static LiveChartsSettings AddSkiaSharp(
            this LiveChartsSettings settings, Action<StyleBuilder<SkiaSharpDrawingContext>> builder = null)
        {
            return settings
                .HasDataFactory(new DataFactory<SkiaSharpDrawingContext>())
                .AddDefaultStyles((StyleBuilder<SkiaSharpDrawingContext> styleBuilder) =>
                {
                    // default settings
                    styleBuilder
                        .UseColors(ColorPacks.MaterialDesign500)
                        .UseSeriesInitializer(new DefaultInitializer());

                    // user defined settings
                    builder?.Invoke(styleBuilder);
                });
        }
    }
}

