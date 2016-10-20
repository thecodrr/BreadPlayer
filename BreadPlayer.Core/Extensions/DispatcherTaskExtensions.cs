/* 
	BreadPlayer. A music player made for Windows 10 store.
    Copyright (C) 2016  theweavrs (Abdullah Atta)

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/
using System;
using System.Threading.Tasks;
using Windows.UI.Core;

public static class DispatcherTaskExtensions
{
    public static async Task<T> RunTaskAsync<T>(this CoreDispatcher dispatcher,
        Func<Task<T>> func, CoreDispatcherPriority priority = CoreDispatcherPriority.Normal)
    {
        var taskCompletionSource = new TaskCompletionSource<T>();
        await dispatcher.RunAsync(priority, async () =>
        {
            try
            {
                taskCompletionSource.SetResult(await func());
            }
            catch (Exception ex)
            {
                taskCompletionSource.SetException(ex);
            }
        });
        return await taskCompletionSource.Task;
    }

    // There is no TaskCompletionSource<void> so we use a bool that we throw away.
    public static async Task RunTaskAsync(this CoreDispatcher dispatcher,
        Func<Task> func, CoreDispatcherPriority priority = CoreDispatcherPriority.Normal) =>
        await RunTaskAsync(dispatcher, async () => { await func(); return false; }, priority);
}