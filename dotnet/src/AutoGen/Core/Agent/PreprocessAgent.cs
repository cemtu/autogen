﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// PreprocessAgent.cs

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.SemanticKernel.ChatCompletion;

namespace AutoGen
{
    /// <summary>
    /// The preprocess agent preprocesses the conversation before passing it to the inner agent by calling the preprocess function.
    /// </summary>
    public class PreProcessAgent : IAgent
    {
        /// <summary>
        /// Create a wrapper agent that preprocesses the conversation before passing it to the inner agent.
        /// </summary>
        /// <param name="innerAgent">inner agent.</param>
        /// <param name="name">name</param>
        /// <param name="preprocessFunc">preprocess function</param>
        public PreProcessAgent(
            IAgent innerAgent,
            string name,
            Func<IEnumerable<Message>, CancellationToken, Task<IEnumerable<Message>>> preprocessFunc)
        {
            InnerAgent = innerAgent;
            Name = name;
            PreprocessFunc = preprocessFunc;
        }

        public IAgent InnerAgent { get; }

        public string Name { get; }

        public Func<IEnumerable<Message>, CancellationToken, Task<IEnumerable<Message>>> PreprocessFunc { get; }

        public IChatCompletionService? ChatCompletion => InnerAgent.ChatCompletion;

        /// <summary>
        /// First preprocess the <paramref name="conversation"/> using the <see cref="PreprocessFunc"/> and then pass the preprocessed conversation to the <see cref="InnerAgent"/>."/>
        /// </summary>
        public async Task<Message> GenerateReplyAsync(IEnumerable<Message> conversation, CancellationToken ct = default)
        {
            var messages = await PreprocessFunc(conversation, ct);
            var reply = await InnerAgent.GenerateReplyAsync(messages, ct);
            reply.From = Name;

            return reply;
        }
    }
}