import 'dart:async';

import 'package:flutter/material.dart';
import 'package:grooveon_mobile/models/notification_response.dart';
import 'package:grooveon_mobile/providers/notification_provider.dart';
import 'package:provider/provider.dart';

class NotificationsScreen extends StatefulWidget {
  final ValueChanged<int>? onUnreadChanged;

  const NotificationsScreen({super.key, this.onUnreadChanged});

  @override
  State<NotificationsScreen> createState() => _NotificationsScreenState();
}

class _NotificationsScreenState extends State<NotificationsScreen> {
  static const Color primary = Color(0xFF9C27B0);
  static const Color bg = Color(0xFFF8F6FB);

  List<NotificationResponse> _items = [];
  bool _isLoading = true;
  String? _error;
  Timer? _timer;

  int get unreadCount => _items.where((x) => !x.isRead).length;

  @override
  void initState() {
    super.initState();
    _load();
    _timer = Timer.periodic(const Duration(seconds: 30), (_) => _load(silent: true));
  }

  @override
  void dispose() {
    _timer?.cancel();
    super.dispose();
  }

  Future<void> _load({bool silent = false}) async {
    try {
      if (!silent && mounted) {
        setState(() {
          _isLoading = true;
          _error = null;
        });
      }

      final result = await context.read<NotificationProvider>().get(filter: {
        "Page": 0,
        "PageSize": 50,
        "IncludeTotalCount": true,
      });

      if (!mounted) return;

      setState(() {
        _items = result.items;
        _isLoading = false;
        _error = null;
      });
      widget.onUnreadChanged?.call(unreadCount);
    } catch (e) {
      if (!mounted) return;
      setState(() {
        _error = e.toString();
        _isLoading = false;
      });
    }
  }

  Future<void> _markAsRead(NotificationResponse item) async {
    if (item.isRead) return;

    final updated = await context.read<NotificationProvider>().markAsRead(item.id);

    if (!mounted) return;

    setState(() {
      _items = _items
          .map((x) => x.id == updated.id ? updated : x)
          .toList(growable: false);
    });
    widget.onUnreadChanged?.call(unreadCount);
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: bg,
      body: SafeArea(
        child: Column(
          children: [
            _header(),
            Expanded(child: _body()),
          ],
        ),
      ),
    );
  }

  Widget _header() {
    return Container(
      width: double.infinity,
      padding: const EdgeInsets.fromLTRB(20, 18, 20, 20),
      decoration: const BoxDecoration(
        gradient: LinearGradient(
          colors: [Color(0xFF4A148C), primary],
          begin: Alignment.topLeft,
          end: Alignment.bottomRight,
        ),
      ),
      child: Row(
        children: [
          const Icon(Icons.notifications_rounded, color: Colors.white, size: 30),
          const SizedBox(width: 12),
          const Expanded(
            child: Text(
              "Notifications",
              style: TextStyle(
                color: Colors.white,
                fontSize: 24,
                fontWeight: FontWeight.w900,
              ),
            ),
          ),
          if (unreadCount > 0)
            Container(
              padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 6),
              decoration: BoxDecoration(
                color: Colors.white,
                borderRadius: BorderRadius.circular(999),
              ),
              child: Text(
                "$unreadCount unread",
                style: const TextStyle(
                  color: primary,
                  fontWeight: FontWeight.w800,
                  fontSize: 12,
                ),
              ),
            ),
        ],
      ),
    );
  }

  Widget _body() {
    if (_isLoading) {
      return const Center(child: CircularProgressIndicator(color: primary));
    }

    if (_error != null) {
      return Center(
        child: IconButton(
          onPressed: _load,
          icon: const Icon(Icons.refresh_rounded, color: primary, size: 34),
        ),
      );
    }

    if (_items.isEmpty) {
      return const Center(
        child: Text(
          "No notifications yet.",
          style: TextStyle(color: Colors.black54, fontWeight: FontWeight.w700),
        ),
      );
    }

    return RefreshIndicator(
      color: primary,
      onRefresh: _load,
      child: ListView.separated(
        padding: const EdgeInsets.fromLTRB(16, 18, 16, 110),
        itemBuilder: (context, index) => _notificationTile(_items[index]),
        separatorBuilder: (context, index) => const SizedBox(height: 10),
        itemCount: _items.length,
      ),
    );
  }

  Widget _notificationTile(NotificationResponse item) {
    final unread = !item.isRead;

    return InkWell(
      onTap: () => _markAsRead(item),
      borderRadius: BorderRadius.circular(14),
      child: Container(
        padding: const EdgeInsets.all(14),
        decoration: BoxDecoration(
          color: Colors.white,
          borderRadius: BorderRadius.circular(14),
          border: Border.all(
            color: unread ? const Color(0x559C27B0) : const Color(0xFFE8E0EC),
          ),
          boxShadow: const [
            BoxShadow(
              color: Color(0x0D000000),
              blurRadius: 10,
              offset: Offset(0, 4),
            ),
          ],
        ),
        child: Row(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Container(
              width: 42,
              height: 42,
              decoration: BoxDecoration(
                color: unread ? primary : const Color(0xFFEDE7F6),
                shape: BoxShape.circle,
              ),
              child: Icon(
                _iconForType(item.type),
                color: unread ? Colors.white : primary,
              ),
            ),
            const SizedBox(width: 12),
            Expanded(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(
                    item.title,
                    style: TextStyle(
                      color: Colors.black87,
                      fontSize: 15,
                      fontWeight: unread ? FontWeight.w900 : FontWeight.w700,
                    ),
                  ),
                  const SizedBox(height: 5),
                  Text(
                    item.content,
                    style: const TextStyle(
                      color: Colors.black54,
                      height: 1.35,
                    ),
                  ),
                ],
              ),
            ),
            if (unread) ...[
              const SizedBox(width: 8),
              const Icon(Icons.circle, color: primary, size: 10),
            ],
          ],
        ),
      ),
    );
  }

  IconData _iconForType(String type) {
    switch (type) {
      case "question_answered":
        return Icons.question_answer_rounded;
      case "payment_failed":
        return Icons.error_rounded;
      case "payment_canceled":
        return Icons.cancel_rounded;
      default:
        return Icons.notifications_rounded;
    }
  }
}
