import 'package:flutter/material.dart';
import 'package:grooveon_mobile/helper/universal_paging_helper.dart';
import 'package:grooveon_mobile/models/question_response.dart';
import 'package:grooveon_mobile/models/search_results.dart';
import 'package:grooveon_mobile/providers/question_provider.dart';
import 'package:grooveon_mobile/screens/ask_question_screen.dart';
import 'package:grooveon_mobile/utils/Session.dart';
import 'package:grooveon_mobile/widgets/swipe_widget.dart';

class MyQuestionsScreen extends StatefulWidget {
  const MyQuestionsScreen({super.key});

  @override
  State<MyQuestionsScreen> createState() => _MyQuestionsScreenState();
}

class _MyQuestionsScreenState extends State<MyQuestionsScreen>
    with SingleTickerProviderStateMixin {
  static const Color _primary = Color(0xFF9C27B0);
  static const Color _primaryDark = Color(0xFF4A148C);
  static const Color _softPurple = Color(0xFFF3E5F5);
  static const Color _deepPurple = Color(0xFF6A1B9A);
  static const Color _pendingPurple = Color(0xFF7E57C2);
  static const Color _answeredPurple = Color(0xFFBA68C8);

  static const Color _bg = Color(0xFFF8F6FB);
  static const Color _card = Colors.white;
  static const Color _textDark = Color(0xFF1C1C1C);
  static const Color _textLight = Color(0xFF777784);

  late final TabController _tabController;
  late final QuestionProvider _questionProvider;

  late final UniversalPagingProvider<QuestionResponse> _pendingPaging;
  late final UniversalPagingProvider<QuestionResponse> _answeredPaging;

  bool _loading = true;
  String? _error;

  @override
  void initState() {
    super.initState();

    _tabController = TabController(length: 2, vsync: this);
    _questionProvider = QuestionProvider();

    _pendingPaging = UniversalPagingProvider<QuestionResponse>(
      pageSize: 10,
      fetcher: ({
        required int page,
        required int pageSize,
        String? filter,
        Map<String, dynamic>? extra,
        bool includeTotalCount = true,
      }) async {
        final result = await _questionProvider.get(filter: {
          "UserId": Session.userId,
          "Status": "Pending",
          "Page": page,
          "PageSize": pageSize,
          "IncludeTotalCount": includeTotalCount,
          ...?extra,
        });

        return SearchResult<QuestionResponse>(
          items: result.items,
          totalCount: result.totalCount ?? 0,
        );
      },
    );

    _answeredPaging = UniversalPagingProvider<QuestionResponse>(
      pageSize: 10,
      fetcher: ({
        required int page,
        required int pageSize,
        String? filter,
        Map<String, dynamic>? extra,
        bool includeTotalCount = true,
      }) async {
        final result = await _questionProvider.get(filter: {
          "UserId": Session.userId,
          "Status": "Answered",
          "Page": page,
          "PageSize": pageSize,
          "IncludeTotalCount": includeTotalCount,
          ...?extra,
        });

        return SearchResult<QuestionResponse>(
          items: result.items,
          totalCount: result.totalCount ?? 0,
        );
      },
    );

    _loadData();
  }

  Future<void> _loadData() async {
    try {
      setState(() {
        _loading = true;
        _error = null;
      });

      await Future.wait([
        _pendingPaging.loadPage(pageNumber: 0),
        _answeredPaging.loadPage(pageNumber: 0),
      ]);

      if (!mounted) return;
      setState(() => _loading = false);
    } catch (e) {
      if (!mounted) return;
      setState(() {
        _error = e.toString();
        _loading = false;
      });
    }
  }

  Future<void> _refreshCurrentTab() async {
    if (_tabController.index == 0) {
      await _pendingPaging.refresh();
    } else {
      await _answeredPaging.refresh();
    }

    if (mounted) setState(() {});
  }

  Future<void> _openAskDialog() async {
    final created = await showDialog<bool>(
      context: context,
      builder: (_) => const AskQuestionDialog(),
    );

    if (created == true) {
      await Future.wait([
        _pendingPaging.refresh(),
        _answeredPaging.refresh(),
      ]);

      if (mounted) setState(() {});
    }
  }

  @override
  void dispose() {
    _tabController.dispose();
    _pendingPaging.dispose();
    _answeredPaging.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: _bg,
      floatingActionButton: FloatingActionButton.extended(
        onPressed: _openAskDialog,
        backgroundColor: _primary,
        foregroundColor: Colors.white,
        icon: const Icon(Icons.add_rounded),
        label: const Text(
          "Ask Question",
          style: TextStyle(fontWeight: FontWeight.w800),
        ),
      ),
      body: SafeArea(
        child: _loading
            ? const Center(child: CircularProgressIndicator(color: _primary))
            : _error != null
                ? _errorState()
                : Column(
                    children: [
                      _header(),
                      _tabs(),
                      Expanded(
                        child: RefreshIndicator(
                          onRefresh: _refreshCurrentTab,
                          color: _primary,
                          child: TabBarView(
                            controller: _tabController,
                            children: [
                              _questionsList(_pendingPaging, false),
                              _questionsList(_answeredPaging, true),
                            ],
                          ),
                        ),
                      ),
                    ],
                  ),
      ),
    );
  }

  Widget _header() {
    return Container(
      padding: const EdgeInsets.fromLTRB(14, 12, 18, 18),
      decoration: const BoxDecoration(
        gradient: LinearGradient(
          colors: [_primaryDark, _primary],
          begin: Alignment.topLeft,
          end: Alignment.bottomRight,
        ),
      ),
      child: Column(
        children: [
          Row(
            children: [
              IconButton(
                onPressed: () => Navigator.pop(context),
                icon: const Icon(
                  Icons.arrow_back_ios_new_rounded,
                  color: Colors.white,
                ),
              ),
              const SizedBox(width: 6),
              const Expanded(
                child: Text(
                  "My Questions",
                  style: TextStyle(
                    color: Colors.white,
                    fontSize: 24,
                    fontWeight: FontWeight.w900,
                  ),
                ),
              ),
            ],
          ),
          const SizedBox(height: 8),
          const Align(
            alignment: Alignment.centerLeft,
            child: Padding(
              padding: EdgeInsets.only(left: 8),
              child: Text(
                "Track your answered and pending GrooveOn questions.",
                style: TextStyle(
                  color: Colors.white70,
                  fontSize: 14,
                  fontWeight: FontWeight.w600,
                ),
              ),
            ),
          ),
        ],
      ),
    );
  }

  Widget _tabs() {
    return Container(
      margin: const EdgeInsets.fromLTRB(18, 16, 18, 10),
      padding: const EdgeInsets.all(5),
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(18),
      ),
      child: TabBar(
        controller: _tabController,
        indicator: BoxDecoration(
          color: _primary.withOpacity(0.12),
          borderRadius: BorderRadius.circular(14),
        ),
        labelColor: _primary,
        unselectedLabelColor: _textLight,
        labelStyle: const TextStyle(
          fontWeight: FontWeight.w900,
          fontSize: 14,
        ),
        unselectedLabelStyle: const TextStyle(
          fontWeight: FontWeight.w700,
          fontSize: 14,
        ),
        tabs: [
          Tab(text: "Pending (${_pendingPaging.totalCount})"),
          Tab(text: "Answered (${_answeredPaging.totalCount})"),
        ],
      ),
    );
  }

  Widget _questionsList(
    UniversalPagingProvider<QuestionResponse> provider,
    bool answered,
  ) {
    if (provider.items.isEmpty) {
      return ListView(
        physics: const AlwaysScrollableScrollPhysics(),
        padding: const EdgeInsets.fromLTRB(24, 80, 24, 120),
        children: [
          Icon(
            answered
                ? Icons.mark_chat_read_rounded
                : Icons.hourglass_empty_rounded,
            color: _primary.withOpacity(0.5),
            size: 56,
          ),
          const SizedBox(height: 14),
          Text(
            answered ? "No answered questions yet." : "No pending questions.",
            textAlign: TextAlign.center,
            style: const TextStyle(
              color: _textDark,
              fontSize: 17,
              fontWeight: FontWeight.w900,
            ),
          ),
          const SizedBox(height: 8),
          const Text(
            "You can ask a new question using the button below.",
            textAlign: TextAlign.center,
            style: TextStyle(
              color: _textLight,
              fontSize: 14,
              height: 1.35,
            ),
          ),
        ],
      );
    }

    return ListView(
      physics: const AlwaysScrollableScrollPhysics(),
      padding: const EdgeInsets.fromLTRB(18, 6, 18, 120),
      children: [
        SwipePagedList<QuestionResponse>(
          provider: provider,
          separatorHeight: 12,
          itemBuilder: (context, question) => _questionCard(question),
        ),
      ],
    );
  }

  Widget _questionCard(QuestionResponse question) {
    final answered =
        question.answer != null && question.answer!.trim().isNotEmpty;

    final statusColor = answered ? _answeredPurple : _pendingPurple;

    return InkWell(
      onTap: () => _openQuestionDetails(question),
      borderRadius: BorderRadius.circular(20),
      child: Container(
        padding: const EdgeInsets.all(16),
        decoration: BoxDecoration(
          color: _card,
          borderRadius: BorderRadius.circular(20),
          border: Border.all(
            color: statusColor.withOpacity(0.28),
          ),
          boxShadow: [
            BoxShadow(
              color: statusColor.withOpacity(0.08),
              blurRadius: 12,
              offset: const Offset(0, 5),
            ),
          ],
        ),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Row(
              children: [
                Expanded(
                  child: Text(
                    question.title,
                    maxLines: 1,
                    overflow: TextOverflow.ellipsis,
                    style: const TextStyle(
                      color: _textDark,
                      fontSize: 16,
                      fontWeight: FontWeight.w900,
                    ),
                  ),
                ),
                _statusChip(answered),
              ],
            ),
            const SizedBox(height: 8),
            Text(
              question.content,
              maxLines: 2,
              overflow: TextOverflow.ellipsis,
              style: const TextStyle(
                color: _textLight,
                fontSize: 13.5,
                height: 1.35,
                fontWeight: FontWeight.w500,
              ),
            ),
            const SizedBox(height: 12),
            Row(
              children: [
                Icon(
                  answered
                      ? Icons.chat_bubble_rounded
                      : Icons.schedule_rounded,
                  color: statusColor,
                  size: 17,
                ),
                const SizedBox(width: 6),
                Text(
                  answered ? "Tap to see answer" : "Waiting for admin answer",
                  style: TextStyle(
                    color: statusColor,
                    fontSize: 12.5,
                    fontWeight: FontWeight.w800,
                  ),
                ),
              ],
            ),
          ],
        ),
      ),
    );
  }

  Widget _statusChip(bool answered) {
    final statusColor = answered ? _answeredPurple : _pendingPurple;

    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 5),
      decoration: BoxDecoration(
        color: statusColor.withOpacity(0.13),
        borderRadius: BorderRadius.circular(30),
        border: Border.all(
          color: statusColor.withOpacity(0.18),
        ),
      ),
      child: Text(
        answered ? "Answered" : "Pending",
        style: TextStyle(
          color: statusColor,
          fontSize: 11,
          fontWeight: FontWeight.w900,
        ),
      ),
    );
  }

  void _openQuestionDetails(QuestionResponse question) {
    final answered =
        question.answer != null && question.answer!.trim().isNotEmpty;

    showModalBottomSheet(
      context: context,
      backgroundColor: Colors.transparent,
      isScrollControlled: true,
      builder: (_) {
        return Container(
          padding: EdgeInsets.fromLTRB(
            20,
            18,
            20,
            MediaQuery.of(context).viewInsets.bottom + 24,
          ),
          decoration: const BoxDecoration(
            color: Colors.white,
            borderRadius: BorderRadius.vertical(
              top: Radius.circular(28),
            ),
          ),
          child: SafeArea(
            top: false,
            child: SingleChildScrollView(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Center(
                    child: Container(
                      width: 44,
                      height: 5,
                      decoration: BoxDecoration(
                        color: Colors.grey.shade300,
                        borderRadius: BorderRadius.circular(20),
                      ),
                    ),
                  ),
                  const SizedBox(height: 18),
                  Row(
                    children: [
                      Expanded(
                        child: Text(
                          question.title,
                          style: const TextStyle(
                            color: _textDark,
                            fontSize: 20,
                            fontWeight: FontWeight.w900,
                          ),
                        ),
                      ),
                      _statusChip(answered),
                    ],
                  ),
                  const SizedBox(height: 14),
                  const Text(
                    "Your question",
                    style: TextStyle(
                      color: _primary,
                      fontSize: 14,
                      fontWeight: FontWeight.w900,
                    ),
                  ),
                  const SizedBox(height: 6),
                  Text(
                    question.content,
                    style: const TextStyle(
                      color: _textDark,
                      fontSize: 14,
                      height: 1.45,
                    ),
                  ),
                  const SizedBox(height: 18),
                  const Text(
                    "Answer",
                    style: TextStyle(
                      color: _primary,
                      fontSize: 14,
                      fontWeight: FontWeight.w900,
                    ),
                  ),
                  const SizedBox(height: 6),
                  Text(
                    answered
                        ? question.answer!.trim()
                        : "Admin has not answered this question yet.",
                    style: TextStyle(
                      color: answered ? _textDark : _textLight,
                      fontSize: 14,
                      height: 1.45,
                      fontStyle:
                          answered ? FontStyle.normal : FontStyle.italic,
                    ),
                  ),
                ],
              ),
            ),
          ),
        );
      },
    );
  }

  Widget _errorState() {
    return Center(
      child: Padding(
        padding: const EdgeInsets.all(24),
        child: Text(
          _error ?? "Failed to load questions.",
          textAlign: TextAlign.center,
          style: const TextStyle(
            color: _primaryDark,
            fontWeight: FontWeight.w700,
          ),
        ),
      ),
    );
  }
}