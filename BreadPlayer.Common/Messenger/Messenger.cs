using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace BreadPlayer.Messengers
{
    public enum MessageHandledStatus
    {
        NotHandled,         // The message has not been handled
        HandledContinue,    // I have handled the message, but let others know about it
        HandledCompleted,   // I have handled the message but tell nobody else about it
        NotHandledAbort     // I haven't handled the message but want to abort it anyway
    }

    public enum NotificationResult
    {
        MessageNotRegistered,       // The message had no handlers registered
        MessageHandled,             // The message had one or more handlers, and was handled
        MessageAborted,             // The message had a handler who aborted the message
        MessageRegisteredNotHandled // Although the message had one or more handlers, none of them appeared to handle the message
    }

    /// <summary>
    /// Provides loosely-coupled messaging between
    /// various colleague objects.  All references to objects
    /// are stored weakly, to prevent memory leaks.
    /// Based on (stolen from!) the MVVMFoundation Messenger class - but modified to use an enumeration rather than a string
    /// </summary>
    public class Messenger
    {
        private static readonly Messenger instance = new Messenger();

        #region Constructor

        /// <summary>
        /// Private constructor for this singleton - use Messenger.Instance
        /// </summary>
        private Messenger()
        {
        }

        #endregion Constructor

        #region Public Properties

        public static Messenger Instance => instance;

        #endregion Public Properties

        #region Register

        /// <summary>
        /// Registers a call-back method, with no parameter, to be invoked when a specific message is broadcast.
        /// </summary>
        /// <param name="message">The message to register for.</param>
        /// <param name="callback">The call-back to be called when this message is broadcasted.</param>
        public void Register(MessageTypes message, Action callback)
        {
            Register(message, callback, null);
        }

        /// <summary>
        /// Registers a call-back method, with a parameter, to be invoked when a specific message is broadcast.
        /// </summary>
        /// <param name="message">The message to register for.</param>
        /// <param name="callback">The call-back to be called when this message is broadcasted.</param>
        public void Register<T>(MessageTypes message, Action<T> callback)
        {
            Register(message, callback, typeof(T));
        }

        private void Register(MessageTypes message, Delegate callback, Type parameterType)
        {
            if (callback == null)
            {
                throw new ArgumentNullException("callback");
            }

            VerifyParameterType(message, parameterType);

            _messageToActionsMap.AddAction(message, callback.Target, callback.GetMethodInfo(), parameterType);
        }

        /// <summary>
        /// When your class is listening for a message with a particular method, the DeRegister method will
        /// remove your delagte from the collection - so this particular listener will no lionger receive this particular message
        /// </summary>
        /// <param name="message"></param>
        /// <param name="callback"></param>
        public void DeRegister(MessageTypes message, Delegate callback)
        {
            _messageToActionsMap.RemoveAction(message, callback.Target, callback.GetMethodInfo());
        }

        /// <summary>
        /// When your class is listening to one or more messages, this DeRegister method will
        /// remove all delegates for that class from the collection.
        /// Should be called when a ViewModel closes to ensure un-Garbage Collected objects don't continue to
        /// receive messages
        /// </summary>
        /// <param name="callback"></param>
        public void DeRegister(object target)
        {
            _messageToActionsMap.RemoveActions(target);
        }

        [Conditional("DEBUG")]
        private void VerifyParameterType(MessageTypes message, Type parameterType)
        {
            if (_messageToActionsMap.TryGetParameterType(message, out Type previouslyRegisteredParameterType))
            {
                if (previouslyRegisteredParameterType != null && parameterType != null)
                {
                    if (!previouslyRegisteredParameterType.Equals(parameterType))
                    {
                        throw new InvalidOperationException(string.Format(
                            "The registered action's parameter type is inconsistent with the previously registered actions for message '{0}'.\nExpected: {1}\nAdding: {2}",
                            message,
                            previouslyRegisteredParameterType.FullName,
                            parameterType.FullName));
                    }
                }
                else
                {
                    // One, or both, of previouslyRegisteredParameterType or callbackParameterType are null.
                    if (previouslyRegisteredParameterType != parameterType)   // not both null?
                    {
                        throw new TargetParameterCountException(string.Format(
                            "The registered action has a number of parameters inconsistent with the previously registered actions for message \"{0}\".\nExpected: {1}\nAdding: {2}",
                            message,
                            previouslyRegisteredParameterType == null ? 0 : 1,
                            parameterType == null ? 0 : 1));
                    }
                }
            }
        }

        #endregion Register

        #region NotifyColleagues

        /// <summary>
        /// Notifies all registered parties that a message is being broadcasted.
        /// </summary>
        /// <param name="messageType">The message to broadcast.</param>
        /// <param name="parameter">The parameter to pass together with the message.</param>
        public NotificationResult NotifyColleagues(MessageTypes messageType, object parameter)
        {
            //if (string.IsNullOrEmpty(message))
            //    throw new ArgumentException("'message' cannot be null or empty.");

            if (_messageToActionsMap.TryGetParameterType(messageType, out Type registeredParameterType)
                && registeredParameterType == null)
            {
                throw new TargetParameterCountException(string.Format("Cannot pass a parameter with message '{0}'. Registered action(s) expect no parameter.", messageType));
            }

            var actions = _messageToActionsMap.GetActions(messageType);
            if (actions != null)
            {
                Message message = new Message(messageType)
                {
                    HandledStatus = MessageHandledStatus.NotHandled,
                    Payload = parameter
                };

                foreach (var action in actions)
                {
                    Invoke(message, action);
                }

                switch (message.HandledStatus)
                {
                    case MessageHandledStatus.NotHandled:
                        return NotificationResult.MessageRegisteredNotHandled;

                    case MessageHandledStatus.HandledContinue:
                        return NotificationResult.MessageHandled;

                    case MessageHandledStatus.HandledCompleted:
                        return NotificationResult.MessageHandled;

                    case MessageHandledStatus.NotHandledAbort:
                        return NotificationResult.MessageAborted;

                    default:
                        break;
                }
            }
            return NotificationResult.MessageNotRegistered;
        }

        private void Invoke(Message message, Delegate method)
        {
            if (message.HandledStatus == MessageHandledStatus.HandledContinue || message.HandledStatus == MessageHandledStatus.NotHandled)
            {
                method.DynamicInvoke(message);
            }
        }

        /// <summary>
        /// Notifies all registered parties that a message is being broadcasted.
        /// </summary>
        /// <param name="messageType">The message to broadcast.</param>
        public void NotifyColleagues(MessageTypes messageType)
        {
            //if (string.IsNullOrEmpty(message))
            //    throw new ArgumentException("'message' cannot be null or empty.");

            if (_messageToActionsMap.TryGetParameterType(messageType, out Type registeredParameterType)
                && registeredParameterType != null)
            {
                throw new TargetParameterCountException(string.Format("Must pass a parameter of type {0} with this message. Registered action(s) expect it.", registeredParameterType.FullName));
            }

            var actions = _messageToActionsMap.GetActions(messageType);
            if (actions != null)
            {
                // actions.ForEach(action => action.DynamicInvoke());

                foreach (var action in actions)
                {
                    action.DynamicInvoke();
                }
            }
        }

        #endregion NotifyColleagues

        #region MessageToActionsMap [nested class]

        /// <summary>
        /// This class is an implementation detail of the Messenger class.
        /// </summary>
        private class MessageToActionsMap
        {
            #region AddAction

            /// <summary>
            /// Adds an action to the list.
            /// </summary>
            /// <param name="message">The message to register.</param>
            /// <param name="target">The target object to invoke, or null.</param>
            /// <param name="method">The method to invoke.</param>
            /// <param name="actionType">The type of the Action delegate.</param>
            internal void AddAction(MessageTypes message, object target, MethodInfo method, Type actionType)
            {
                //if (message == null)
                //    throw new ArgumentNullException("message");

                if (method == null)
                {
                    throw new ArgumentNullException("method");
                }

                lock (_map)
                {
                    if (!_map.ContainsKey(message))
                    {
                        _map[message] = new List<WeakAction>();
                    }

                    _map[message].Add(new WeakAction(target, method, actionType));
                }
            }

            /// <summary>
            /// Removes an action from the list, for the message type, object and method specified
            /// </summary>
            /// <param name="message"></param>
            /// <param name="target"></param>
            /// <param name="method"></param>
            /// <param name="actionType"></param>
            internal void RemoveAction(MessageTypes message, object target, MethodInfo method)
            {
                lock (_map)
                {
                    if (_map.TryGetValue(message, out List<WeakAction> wr))
                    {
                        wr.RemoveAll(wa => target == wa.TargetRef.Target && method == wa.Method);

                        if (wr.Count == 0)
                        {
                            _map.Remove(message);
                        }
                    }
                }
            }

            /// <summary>
            /// Remove all actions from the list for the given target object
            /// Used when 'closing' a viewmodel
            /// </summary>
            /// <param name="target"></param>
            internal void RemoveActions(object target)
            {
                lock (_map)
                {
                    foreach (var item in _map.Values)
                    {
                        item.RemoveAll(wa => target == wa.TargetRef.Target);
                    }
                }
            }

            #endregion AddAction

            #region GetActions

            /// <summary>
            /// Gets the list of actions to be invoked for the specified message
            /// </summary>
            /// <param name="message">The message to get the actions for</param>
            /// <returns>Returns a list of actions that are registered to the specified message</returns>
            internal List<Delegate> GetActions(MessageTypes message)
            {
                //if (message == null)
                //    throw new ArgumentNullException("message");

                List<Delegate> actions;
                lock (_map)
                {
                    if (!_map.ContainsKey(message))
                    {
                        return null;
                    }

                    List<WeakAction> weakActions = _map[message];
                    actions = new List<Delegate>(weakActions.Count);
                    for (int i = weakActions.Count - 1; i > -1; --i)
                    {
                        WeakAction weakAction = weakActions[i];
                        if (weakAction == null)
                        {
                            continue;
                        }

                        Delegate action = weakAction.CreateAction();
                        if (action != null)
                        {
                            actions.Add(action);
                        }
                        else
                        {
                            // The target object is dead, so get rid of the weak action.
                            weakActions.Remove(weakAction);
                        }
                    }

                    // Delete the list from the map if it is now empty.
                    if (weakActions.Count == 0)
                    {
                        _map.Remove(message);
                    }
                }

                // Reverse the list to ensure the callbacks are invoked in the order they were registered.
                actions.Reverse();

                return actions;
            }

            #endregion GetActions

            #region TryGetParameterType

            /// <summary>
            /// Get the parameter type of the actions registered for the specified message.
            /// </summary>
            /// <param name="message">The message to check for actions.</param>
            /// <param name="parameterType">
            /// When this method returns, contains the type for parameters
            /// for the registered actions associated with the specified message, if any; otherwise, null.
            /// This will also be null if the registered actions have no parameters.
            /// This parameter is passed uninitialized.
            /// </param>
            /// <returns>true if any actions were registered for the message</returns>
            internal bool TryGetParameterType(MessageTypes message, out Type parameterType)
            {
                //if (message == null)
                //    throw new ArgumentNullException("message");

                parameterType = null;
                List<WeakAction> weakActions;
                lock (_map)
                {
                    if (!_map.TryGetValue(message, out weakActions) || weakActions.Count == 0)
                    {
                        return false;
                    }
                }
                parameterType = weakActions[0].ParameterType;
                return true;
            }

            #endregion TryGetParameterType

            #region Fields

            // Stores a hash where the key is the message and the value is the list of callbacks to invoke.
            private readonly Dictionary<MessageTypes, List<WeakAction>> _map = new Dictionary<MessageTypes, List<WeakAction>>();

            #endregion Fields
        }

        #endregion MessageToActionsMap [nested class]

        #region WeakAction [nested class]

        /// <summary>
        /// This class is an implementation detail of the MessageToActionsMap class.
        /// </summary>
        private class WeakAction
        {
            #region Constructor

            /// <summary>
            /// Constructs a WeakAction.
            /// </summary>
            /// <param name="target">The object on which the target method is invoked, or null if the method is static.</param>
            /// <param name="method">The MethodInfo used to create the Action.</param>
            /// <param name="parameterType">The type of parameter to be passed to the action. Pass null if there is no parameter.</param>
            internal WeakAction(object target, MethodInfo method, Type parameterType)
            {
                if (target == null)
                {
                    _targetRef = null;
                }
                else
                {
                    _targetRef = new WeakReference(target);
                }

                _method = method;

                ParameterType = parameterType;

                if (parameterType == null)
                {
                    _delegateType = typeof(Action);
                }
                else
                {
                    _delegateType = typeof(Action<>).MakeGenericType(parameterType);
                }
            }

            #endregion Constructor

            #region CreateAction

            /// <summary>
            /// Creates a "throw away" delegate to invoke the method on the target, or null if the target object is dead.
            /// </summary>
            internal Delegate CreateAction()
            {
                // Rehydrate into a real Action object, so that the method can be invoked.
                if (_targetRef == null)
                {
                    return _method.CreateDelegate(_delegateType);
                }
                try
                {
                    object target = _targetRef.Target;
                    if (target != null)
                    {
                        return _method.CreateDelegate(_delegateType, target);
                    }
                }
                catch
                {
                }

                return null;
            }

            #endregion CreateAction

            #region Fields

            internal readonly Type ParameterType;

            private readonly Type _delegateType;
            private readonly MethodInfo _method;
            private readonly WeakReference _targetRef;

            #endregion Fields

            public WeakReference TargetRef => _targetRef;
            public MethodInfo Method => _method;
        }

        #endregion WeakAction [nested class]

        #region Fields

        private readonly MessageToActionsMap _messageToActionsMap = new MessageToActionsMap();

        #endregion Fields
    }
}