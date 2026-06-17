import 'package:flutter/material.dart';
import 'package:grooveon_mobile/helper/snackBar_helper.dart';
import 'package:grooveon_mobile/providers/question_provider.dart';
import 'package:grooveon_mobile/utils/Session.dart';
import 'package:provider/provider.dart';
import 'package:grooveon_mobile/helper/exception_read_helper.dart';

class AskQuestionDialog extends StatefulWidget {
  const AskQuestionDialog({super.key});

  @override
  State<AskQuestionDialog> createState() => _AskQuestionDialogState();
}

class _AskQuestionDialogState extends State<AskQuestionDialog> {
  static const Color _primary = Color(0xFF9C27B0);

  final _formKey = GlobalKey<FormState>();
  final _titleController = TextEditingController();
  final _contentController = TextEditingController();

  bool _saving = false;

  @override
  void dispose() {
    _titleController.dispose();
    _contentController.dispose();
    super.dispose();
  }

  Future<void> _submitQuestion() async {
    if (!_formKey.currentState!.validate()) return;

    final userId = Session.userId;
    if (userId == null) {
      SnackbarHelper.showError(context, "User is not logged in.");
      return;
    }

    try {
      setState(() => _saving = true);

      await context.read<QuestionProvider>().insert({
        "userId": userId,
        "title": _titleController.text.trim(),
        "content": _contentController.text.trim(),
      });

      if (!mounted) return;

      Navigator.pop(context, true);
      SnackbarHelper.showSuccess(context, "Question successfully sent!");
    } catch (e) {
      if (!mounted) return;
      SnackbarHelper.showError(context, extractErrorMessage(e));
    } finally {
      if (mounted) setState(() => _saving = false);
    }
  }

  @override
  Widget build(BuildContext context) {
    return Dialog(
      backgroundColor: Colors.transparent,
      insetPadding: const EdgeInsets.symmetric(horizontal: 18, vertical: 22),
      child: Container(
        constraints: const BoxConstraints(maxWidth: 520),
        padding: const EdgeInsets.all(20),
        decoration: BoxDecoration(
          color: Colors.white,
          borderRadius: BorderRadius.circular(26),
          boxShadow: [
            BoxShadow(
              color: _primary.withOpacity(0.18),
              blurRadius: 24,
              offset: const Offset(0, 12),
            ),
          ],
        ),
        child: Form(
          key: _formKey,
          child: SingleChildScrollView(
            child: Column(
              mainAxisSize: MainAxisSize.min,
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Row(
                  children: [
                    const Expanded(
                      child: Text(
                        "Ask a Question",
                        style: TextStyle(
                          color: _primary,
                          fontSize: 22,
                          fontWeight: FontWeight.w900,
                        ),
                      ),
                    ),
                    IconButton(
                      onPressed: _saving ? null : () => Navigator.pop(context),
                      icon: const Icon(Icons.close_rounded),
                    ),
                  ],
                ),
                const SizedBox(height: 6),
                Text(
                  "Ask anything about subscriptions, artists, playlists or payments.",
                  style: TextStyle(
                    color: Colors.grey.shade700,
                    fontSize: 14,
                    height: 1.35,
                  ),
                ),
                const SizedBox(height: 18),
                TextFormField(
                  controller: _titleController,
                  decoration: InputDecoration(
                    hintText: "Enter title...",
                    filled: true,
                    fillColor: _primary.withOpacity(0.08),
                    border: OutlineInputBorder(
                      borderRadius: BorderRadius.circular(16),
                      borderSide: BorderSide.none,
                    ),
                  ),
                  validator: (value) {
                    final text = value?.trim() ?? "";
                    if (text.isEmpty) return "Title is required";
                    if (text.length < 3) return "Title must be at least 3 characters";
                    return null;
                  },
                ),
                const SizedBox(height: 14),
                TextFormField(
                  controller: _contentController,
                  minLines: 5,
                  maxLines: 8,
                  decoration: InputDecoration(
                    hintText: "Type your question...",
                    filled: true,
                    fillColor: _primary.withOpacity(0.08),
                    border: OutlineInputBorder(
                      borderRadius: BorderRadius.circular(16),
                      borderSide: BorderSide.none,
                    ),
                  ),
                  validator: (value) {
                    final text = value?.trim() ?? "";
                    if (text.isEmpty) return "Question content is required";
                    if (text.length < 10) return "Question must be at least 10 characters";
                    return null;
                  },
                ),
                const SizedBox(height: 20),
                SizedBox(
                  width: double.infinity,
                  height: 52,
                  child: ElevatedButton.icon(
                    onPressed: _saving ? null : _submitQuestion,
                    icon: _saving
                        ? const SizedBox(
                            width: 18,
                            height: 18,
                            child: CircularProgressIndicator(
                              strokeWidth: 2,
                              color: Colors.white,
                            ),
                          )
                        : const Icon(Icons.send_rounded),
                    label: Text(
                      _saving ? "Sending..." : "Send Question",
                      style: const TextStyle(
                        fontWeight: FontWeight.w800,
                        fontSize: 15,
                      ),
                    ),
                    style: ElevatedButton.styleFrom(
                      backgroundColor: _primary,
                      foregroundColor: Colors.white,
                      shape: RoundedRectangleBorder(
                        borderRadius: BorderRadius.circular(16),
                      ),
                    ),
                  ),
                ),
              ],
            ),
          ),
        ),
      ),
    );
  }
}