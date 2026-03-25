import 'package:flutter/material.dart';
import 'package:grooveon_desktop/dialogs/base_dialogs_frame.dart';
import 'package:grooveon_desktop/helper/snackbar_helper.dart';
import 'package:grooveon_desktop/helper/univerzal_pagging_helper.dart';
import 'package:grooveon_desktop/models/request/answer_upsert_request.dart';
import 'package:grooveon_desktop/models/request/question_upsert_request.dart';
import 'package:grooveon_desktop/models/response/answer_response.dart';
import 'package:grooveon_desktop/models/response/question_response.dart';
import 'package:grooveon_desktop/models/response/search_result.dart';
import 'package:grooveon_desktop/providers/answer_provider.dart';
import 'package:grooveon_desktop/providers/question_provider.dart';
import 'package:grooveon_desktop/screens/users_screen.dart';
import 'package:grooveon_desktop/utils/session.dart';

class UsersQaContent extends StatefulWidget {
  const UsersQaContent({super.key});

  @override
  State<UsersQaContent> createState() => _UsersQaContentState();
}

class _UsersQaContentState extends State<UsersQaContent> {
  final TextEditingController _searchController = TextEditingController();

  late final QuestionProvider _questionProvider;
  late final AnswerProvider _answerProvider;
  late final UniversalPagingProvider<QuestionResponse> _paging;

  bool _isSavingAnswer = false;

  @override
  void initState() {
    super.initState();

    _questionProvider = QuestionProvider();
    _answerProvider = AnswerProvider();

    _paging = UniversalPagingProvider<QuestionResponse>(
      pageSize: 6,
      fetcher: ({
        required int page,
        required int pageSize,
        String? filter,
        bool includeTotalCount = true,
      }) async {
        final filterMap = <String, dynamic>{
          "page": page,
          "pageSize": pageSize,
          "includeTotalCount": includeTotalCount,
        };

        if (filter != null && filter.trim().isNotEmpty) {
          filterMap["FTS"] = filter.trim();
        }

        final SearchResult<QuestionResponse> result =
            await _questionProvider.get(filter: filterMap);

        return result;
      },
    );

    WidgetsBinding.instance.addPostFrameCallback((_) async {
      await _paging.loadPage();
    });
  }

  @override
  void dispose() {
    _searchController.dispose();
    _paging.dispose();
    super.dispose();
  }

  Future<void> _searchQuestions() async {
    await _paging.search(_searchController.text.trim());
  }

  Future<void> _refreshQuestions() async {
    _searchController.clear();
    await _paging.loadPage(pageNumber: 0, filter: "");
  }

  Future<AnswerResponse?> _getExistingAnswer(int questionId) async {
    try {
      final result = await _answerProvider.get(
        filter: {
          "page": 0,
          "pageSize": 1,
          "questionId": questionId,
          "includeTotalCount": true,
        },
      );

      if (result.items.isEmpty) return null;
      return result.items.first;
    } catch (_) {
      return null;
    }
  }

  Future<void> _openAnswerDialog(QuestionResponse question) async {
    final existingAnswer = await _getExistingAnswer(question.id);

    if (!mounted) return;

    final controller = TextEditingController(
      text: existingAnswer?.message ?? question.answer ?? "",
    );

    await showDialog(
      context: context,
      barrierDismissible: false,
      builder: (_) => StatefulBuilder(
        builder: (context, setDialogState) {
          return BaseDialog(
            title: existingAnswer == null ? "Odgovori na pitanje" : "Uredi odgovor",
            width: 700,
            height: 420,
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                _DialogInfoBlock(
                  label: "Pitanje",
                  value: question.title,
                ),
                const SizedBox(height: 14),
                _DialogInfoBlock(
                  label: "Sadržaj",
                  value: question.content,
                  isBody: true,
                ),
                const SizedBox(height: 18),
                const Text(
                  "Odgovor",
                  style: TextStyle(
                    fontSize: 14,
                    fontWeight: FontWeight.w700,
                    color: UsersScreen.textColor,
                  ),
                ),
                const SizedBox(height: 8),
                Expanded(
                  child: TextField(
                    controller: controller,
                    maxLines: null,
                    expands: true,
                    decoration: InputDecoration(
                      hintText: "Unesi odgovor...",
                      filled: true,
                      fillColor: const Color(0xFFF9F9FB),
                      border: OutlineInputBorder(
                        borderRadius: BorderRadius.circular(12),
                        borderSide: const BorderSide(
                          color: UsersScreen.borderColor,
                        ),
                      ),
                      enabledBorder: OutlineInputBorder(
                        borderRadius: BorderRadius.circular(12),
                        borderSide: const BorderSide(
                          color: UsersScreen.borderColor,
                        ),
                      ),
                      focusedBorder: OutlineInputBorder(
                        borderRadius: BorderRadius.circular(12),
                        borderSide: const BorderSide(
                          color: UsersScreen.primaryColor,
                          width: 1.2,
                        ),
                      ),
                    ),
                  ),
                ),
                const SizedBox(height: 18),
                Row(
                  children: [
                    Expanded(
                      child: OutlinedButton(
                        onPressed: _isSavingAnswer
                            ? null
                            : () => Navigator.of(context).pop(),
                        style: OutlinedButton.styleFrom(
                          minimumSize: const Size.fromHeight(46),
                          side: const BorderSide(
                            color: UsersScreen.borderColor,
                          ),
                        ),
                        child: const Text("Otkaži"),
                      ),
                    ),
                    const SizedBox(width: 12),
                    Expanded(
                      child: ElevatedButton(
                        onPressed: _isSavingAnswer
                            ? null
                            : () async {
                                final message = controller.text.trim();

                                if (message.isEmpty) {
                                  SnackbarHelper.showError(
                                    context,
                                    "Odgovor ne može biti prazan.",
                                  );
                                  return;
                                }

                                setState(() {
                                  _isSavingAnswer = true;
                                });
                                setDialogState(() {});

                                try {
                                  if (existingAnswer == null) {
                                    await _answerProvider.insert(
                                      AnswerUpsertRequest(
                                        questionId: question.id,
                                        adminId: Session.userId!,
                                        message: message,
                                      ).toJson(),
                                    );

                                    await _questionProvider.update(
                                      question.id,
                                      QuestionUpsertRequest(
                                        userId: question.userId,
                                        title: question.title,
                                        content: question.content,
                                        status: "Answered",
                                        answer: message,
                                      ).toJson(),
                                    );

                                    if (!mounted) return;
                                    Navigator.of(context).pop();

                                    await _paging.refresh();

                                    if (!mounted) return;
                                    SnackbarHelper.showSuccess(
                                      context,
                                      "Odgovor je uspješno poslan.",
                                    );
                                  } else {
                                    await _answerProvider.update(
                                      existingAnswer.id,
                                      AnswerUpsertRequest(
                                        questionId: question.id,
                                        adminId: Session.userId!,
                                        message: message,
                                      ).toJson(),
                                    );

                                    await _questionProvider.update(
                                      question.id,
                                      QuestionUpsertRequest(
                                        userId: question.userId,
                                        title: question.title,
                                        content: question.content,
                                        status: "Answered",
                                        answer: message,
                                      ).toJson(),
                                    );

                                    if (!mounted) return;
                                    Navigator.of(context).pop();

                                    await _paging.refresh();

                                    if (!mounted) return;
                                    SnackbarHelper.showUpdate(
                                      context,
                                      "Odgovor je uspješno ažuriran.",
                                    );
                                  }
                                } catch (e) {
                                  if (!mounted) return;
                                  SnackbarHelper.showError(
                                    context,
                                    "Greška: $e",
                                  );
                                } finally {
                                  if (mounted) {
                                    setState(() {
                                      _isSavingAnswer = false;
                                    });
                                  }
                                }
                              },
                        style: ElevatedButton.styleFrom(
                          minimumSize: const Size.fromHeight(46),
                          backgroundColor: UsersScreen.primaryColor,
                          foregroundColor: Colors.white,
                          elevation: 0,
                        ),
                        child: _isSavingAnswer
                            ? const SizedBox(
                                width: 18,
                                height: 18,
                                child: CircularProgressIndicator(
                                  strokeWidth: 2,
                                  color: Colors.white,
                                ),
                              )
                            : Text(existingAnswer == null ? "Pošalji odgovor" : "Sačuvaj izmjene"),
                      ),
                    ),
                  ],
                ),
              ],
            ),
          );
        },
      ),
    );

    controller.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return AnimatedBuilder(
      animation: _paging,
      builder: (context, _) {
        return Container(
          width: double.infinity,
          padding: const EdgeInsets.all(24),
          decoration: BoxDecoration(
            color: UsersScreen.cardColor,
            border: Border.all(color: UsersScreen.borderColor),
            borderRadius: BorderRadius.circular(14),
          ),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Row(
                children: [
                  const Expanded(
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        Text(
                          "Q&A",
                          style: TextStyle(
                            fontSize: 30,
                            fontWeight: FontWeight.w800,
                            color: UsersScreen.textColor,
                          ),
                        ),
                        SizedBox(height: 6),
                        Text(
                          "Pregled svih pitanja korisnika i upravljanje odgovorima administratora.",
                          style: TextStyle(
                            fontSize: 14,
                            color: UsersScreen.subTextColor,
                          ),
                        ),
                      ],
                    ),
                  ),
                  _RefreshIconButton(
                    isLoading: _paging.isLoading,
                    onPressed: _refreshQuestions,
                  ),
                ],
              ),
              const SizedBox(height: 18),
              Row(
                children: [
                  Expanded(
                    child: _SearchBox(
                      controller: _searchController,
                      hintText: "Pretraži pitanja...",
                      onSubmitted: (_) => _searchQuestions(),
                    ),
                  ),
                  const SizedBox(width: 12),
                  SizedBox(
                    height: 46,
                    child: ElevatedButton.icon(
                      onPressed: _paging.isLoading ? null : _searchQuestions,
                      icon: const Icon(Icons.search_rounded, size: 18),
                      label: const Text("Search"),
                      style: ElevatedButton.styleFrom(
                        backgroundColor: UsersScreen.primaryColor,
                        foregroundColor: Colors.white,
                        elevation: 0,
                        padding: const EdgeInsets.symmetric(horizontal: 18),
                        shape: RoundedRectangleBorder(
                          borderRadius: BorderRadius.circular(12),
                        ),
                      ),
                    ),
                  ),
                ],
              ),
              const SizedBox(height: 18),
              Expanded(
                child: _buildBody(),
              ),
              const SizedBox(height: 16),
              _PagingControls(
                page: _paging.page,
                pageSize: _paging.pageSize,
                totalCount: _paging.totalCount,
                hasPreviousPage: _paging.hasPreviousPage,
                hasNextPage: _paging.hasNextPage,
                isLoading: _paging.isLoading,
                onPrevious: _paging.previousPage,
                onNext: _paging.nextPage,
              ),
            ],
          ),
        );
      },
    );
  }

  Widget _buildBody() {
    if (_paging.isLoading && _paging.items.isEmpty) {
      return const Center(
        child: CircularProgressIndicator(),
      );
    }

    if (_paging.items.isEmpty) {
      return const _EmptyState(
        title: "Nema pitanja",
        subtitle: "Trenutno nema dostupnih pitanja za prikaz.",
      );
    }

    return ListView.separated(
      itemCount: _paging.items.length,
      separatorBuilder: (_, __) => const SizedBox(height: 12),
      itemBuilder: (context, index) {
        final question = _paging.items[index];

        return _QaItem(
          question: question,
          onAnswer: () => _openAnswerDialog(question),
        );
      },
    );
  }
}

class _QaItem extends StatelessWidget {
  final QuestionResponse question;
  final VoidCallback onAnswer;

  const _QaItem({
    required this.question,
    required this.onAnswer,
  });

  bool get _hasAnswer =>
      question.answer != null && question.answer!.trim().isNotEmpty;

  @override
  Widget build(BuildContext context) {
    final statusText = question.status.trim().isEmpty
        ? (_hasAnswer ? "Answered" : "Pending")
        : question.status;

    final isAnswered = _hasAnswer || statusText.toLowerCase() == "answered";

    return Container(
      width: double.infinity,
      padding: const EdgeInsets.all(16),
      decoration: BoxDecoration(
        color: const Color(0xFFF9F9FB),
        border: Border.all(color: UsersScreen.borderColor),
        borderRadius: BorderRadius.circular(10),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Wrap(
            runSpacing: 10,
            spacing: 10,
            crossAxisAlignment: WrapCrossAlignment.center,
            children: [
              Container(
                padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 6),
                decoration: BoxDecoration(
                  color: isAnswered
                      ? const Color(0xFFEFFAF1)
                      : const Color(0xFFFFF5E8),
                  borderRadius: BorderRadius.circular(999),
                ),
                child: Text(
                  statusText,
                  style: TextStyle(
                    fontSize: 11,
                    fontWeight: FontWeight.w700,
                    color: isAnswered
                        ? const Color(0xFF2E7D32)
                        : const Color(0xFFE67E22),
                  ),
                ),
              ),
              if (question.userName != null && question.userName!.trim().isNotEmpty)
                Text(
                  "User: ${question.userName!}",
                  style: const TextStyle(
                    fontSize: 12,
                    color: UsersScreen.subTextColor,
                    fontWeight: FontWeight.w600,
                  ),
                ),
              Text(
                "Created: ${_formatDate(question.createdAt)}",
                style: const TextStyle(
                  fontSize: 12,
                  color: UsersScreen.subTextColor,
                ),
              ),
            ],
          ),
          const SizedBox(height: 14),
          Text(
            question.title,
            style: const TextStyle(
              fontSize: 15,
              fontWeight: FontWeight.w700,
              color: UsersScreen.textColor,
            ),
          ),
          const SizedBox(height: 8),
          Text(
            question.content,
            style: const TextStyle(
              fontSize: 13,
              color: UsersScreen.subTextColor,
              height: 1.5,
            ),
          ),
          const SizedBox(height: 14),
          Container(
            width: double.infinity,
            padding: const EdgeInsets.all(14),
            decoration: BoxDecoration(
              color: Colors.white,
              border: Border.all(color: UsersScreen.borderColor),
              borderRadius: BorderRadius.circular(10),
            ),
            child: _hasAnswer
                ? Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      const Text(
                        "Answer",
                        style: TextStyle(
                          fontSize: 13,
                          fontWeight: FontWeight.w700,
                          color: UsersScreen.textColor,
                        ),
                      ),
                      const SizedBox(height: 8),
                      Text(
                        question.answer!,
                        style: const TextStyle(
                          fontSize: 13,
                          color: UsersScreen.subTextColor,
                          height: 1.5,
                        ),
                      ),
                      if (question.answeredAt != null) ...[
                        const SizedBox(height: 10),
                        Text(
                          "Answered: ${_formatDate(question.answeredAt!)}",
                          style: const TextStyle(
                            fontSize: 12,
                            color: UsersScreen.subTextColor,
                          ),
                        ),
                      ],
                    ],
                  )
                : const Text(
                    "Još nema odgovora na ovo pitanje.",
                    style: TextStyle(
                      fontSize: 13,
                      color: UsersScreen.subTextColor,
                    ),
                  ),
          ),
          const SizedBox(height: 14),
          Align(
            alignment: Alignment.centerRight,
            child: ElevatedButton.icon(
              onPressed: onAnswer,
              icon: Icon(
                _hasAnswer ? Icons.edit_rounded : Icons.reply_rounded,
                size: 18,
              ),
              label: Text(_hasAnswer ? "Uredi odgovor" : "Odgovori"),
              style: ElevatedButton.styleFrom(
                backgroundColor: UsersScreen.primaryColor,
                foregroundColor: Colors.white,
                elevation: 0,
              ),
            ),
          ),
        ],
      ),
    );
  }

  static String _formatDate(DateTime value) {
    final d = value.toLocal();
    final day = d.day.toString().padLeft(2, '0');
    final month = d.month.toString().padLeft(2, '0');
    final year = d.year.toString();
    final hour = d.hour.toString().padLeft(2, '0');
    final minute = d.minute.toString().padLeft(2, '0');
    return "$day.$month.$year $hour:$minute";
  }
}

class _DialogInfoBlock extends StatelessWidget {
  final String label;
  final String value;
  final bool isBody;

  const _DialogInfoBlock({
    required this.label,
    required this.value,
    this.isBody = false,
  });

  @override
  Widget build(BuildContext context) {
    return Container(
      width: double.infinity,
      padding: const EdgeInsets.all(14),
      decoration: BoxDecoration(
        color: const Color(0xFFF9F9FB),
        border: Border.all(color: UsersScreen.borderColor),
        borderRadius: BorderRadius.circular(12),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text(
            label,
            style: const TextStyle(
              fontSize: 12,
              fontWeight: FontWeight.w700,
              color: UsersScreen.subTextColor,
            ),
          ),
          const SizedBox(height: 6),
          Text(
            value,
            style: TextStyle(
              fontSize: isBody ? 13 : 14,
              fontWeight: isBody ? FontWeight.w400 : FontWeight.w700,
              color: UsersScreen.textColor,
              height: 1.5,
            ),
          ),
        ],
      ),
    );
  }
}

class _SearchBox extends StatelessWidget {
  final TextEditingController controller;
  final String hintText;
  final ValueChanged<String>? onSubmitted;

  const _SearchBox({
    required this.controller,
    required this.hintText,
    this.onSubmitted,
  });

  @override
  Widget build(BuildContext context) {
    return TextField(
      controller: controller,
      onSubmitted: onSubmitted,
      decoration: InputDecoration(
        hintText: hintText,
        prefixIcon: const Icon(Icons.search_rounded),
        filled: true,
        fillColor: const Color(0xFFF8F8FA),
        contentPadding: const EdgeInsets.symmetric(
          horizontal: 14,
          vertical: 14,
        ),
        border: OutlineInputBorder(
          borderRadius: BorderRadius.circular(12),
          borderSide: const BorderSide(color: UsersScreen.borderColor),
        ),
        enabledBorder: OutlineInputBorder(
          borderRadius: BorderRadius.circular(12),
          borderSide: const BorderSide(color: UsersScreen.borderColor),
        ),
        focusedBorder: OutlineInputBorder(
          borderRadius: BorderRadius.circular(12),
          borderSide: const BorderSide(
            color: UsersScreen.primaryColor,
            width: 1.2,
          ),
        ),
      ),
    );
  }
}

class _PagingControls extends StatelessWidget {
  final int page;
  final int pageSize;
  final int totalCount;
  final bool hasPreviousPage;
  final bool hasNextPage;
  final bool isLoading;
  final Future<void> Function() onPrevious;
  final Future<void> Function() onNext;

  const _PagingControls({
    required this.page,
    required this.pageSize,
    required this.totalCount,
    required this.hasPreviousPage,
    required this.hasNextPage,
    required this.isLoading,
    required this.onPrevious,
    required this.onNext,
  });

  @override
  Widget build(BuildContext context) {
    final from = totalCount == 0 ? 0 : (page * pageSize) + 1;
    final to = ((page + 1) * pageSize) > totalCount
        ? totalCount
        : ((page + 1) * pageSize);

    return Row(
      children: [
        Text(
          totalCount == 0 ? "No records" : "Showing $from-$to of $totalCount",
          style: const TextStyle(
            fontSize: 12,
            color: UsersScreen.subTextColor,
          ),
        ),
        const Spacer(),
        OutlinedButton.icon(
          onPressed: (!hasPreviousPage || isLoading) ? null : () => onPrevious(),
          icon: const Icon(Icons.chevron_left_rounded),
          label: const Text("Previous"),
          style: OutlinedButton.styleFrom(
            side: const BorderSide(color: UsersScreen.borderColor),
          ),
        ),
        const SizedBox(width: 8),
        OutlinedButton.icon(
          onPressed: (!hasNextPage || isLoading) ? null : () => onNext(),
          icon: const Icon(Icons.chevron_right_rounded),
          label: const Text("Next"),
          style: OutlinedButton.styleFrom(
            side: const BorderSide(color: UsersScreen.borderColor),
          ),
        ),
      ],
    );
  }
}

class _RefreshIconButton extends StatelessWidget {
  final VoidCallback? onPressed;
  final bool isLoading;

  const _RefreshIconButton({
    required this.onPressed,
    this.isLoading = false,
  });

  @override
  Widget build(BuildContext context) {
    return SizedBox(
      width: 46,
      height: 46,
      child: OutlinedButton(
        onPressed: isLoading ? null : onPressed,
        style: OutlinedButton.styleFrom(
          padding: EdgeInsets.zero,
          side: const BorderSide(color: UsersScreen.borderColor),
          shape: RoundedRectangleBorder(
            borderRadius: BorderRadius.circular(12),
          ),
          backgroundColor: Colors.white,
        ),
        child: isLoading
            ? const SizedBox(
                width: 18,
                height: 18,
                child: CircularProgressIndicator(strokeWidth: 2),
              )
            : const Icon(
                Icons.refresh_rounded,
                size: 20,
                color: UsersScreen.textColor,
              ),
      ),
    );
  }
}

class _EmptyState extends StatelessWidget {
  final String title;
  final String subtitle;

  const _EmptyState({
    required this.title,
    required this.subtitle,
  });

  @override
  Widget build(BuildContext context) {
    return Center(
      child: Padding(
        padding: const EdgeInsets.all(24),
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            const Icon(
              Icons.question_answer_outlined,
              size: 46,
              color: UsersScreen.subTextColor,
            ),
            const SizedBox(height: 14),
            Text(
              title,
              style: const TextStyle(
                fontSize: 16,
                fontWeight: FontWeight.w800,
                color: UsersScreen.textColor,
              ),
            ),
            const SizedBox(height: 8),
            Text(
              subtitle,
              textAlign: TextAlign.center,
              style: const TextStyle(
                fontSize: 13,
                height: 1.5,
                color: UsersScreen.subTextColor,
              ),
            ),
          ],
        ),
      ),
    );
  }
}