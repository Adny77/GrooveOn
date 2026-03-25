import 'package:flutter/material.dart';
import 'package:grooveon_desktop/helper/univerzal_pagging_helper.dart';
import 'package:grooveon_desktop/models/response/search_result.dart';
import 'package:grooveon_desktop/providers/user_provider.dart';
import 'package:grooveon_desktop/screens/users_screen.dart';

class UsersDataContent extends StatefulWidget {
  const UsersDataContent({super.key});

  @override
  State<UsersDataContent> createState() => _UsersDataContentState();
}

class _UsersDataContentState extends State<UsersDataContent> {
  final TextEditingController _searchController = TextEditingController();

  late final UserProvider _userProvider;
  late final UniversalPagingProvider<dynamic> _paging;

  @override
  void initState() {
    super.initState();

    _userProvider = UserProvider();

    _paging = UniversalPagingProvider<dynamic>(
      pageSize: 8,
      fetcher:
          ({
            required int page,
            required int pageSize,
            String? filter,
            bool includeTotalCount = true,
          }) async {
            final map = {
              "page": page,
              "pageSize": pageSize,
              "includeTotalCount": includeTotalCount,
            };

            if (filter != null && filter.trim().isNotEmpty) {
              map["FTS"] = filter.trim();
            }

            final SearchResult result = await _userProvider.get(filter: map);

            return SearchResult<dynamic>(
              items: List<dynamic>.from(result.items),
              totalCount: result.totalCount,
            );
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

  Future<void> _searchUsers() async {
    await _paging.search(_searchController.text.trim());
  }

  Future<void> _refreshUsers() async {
    _searchController.clear();
    await _paging.loadPage(pageNumber: 0, filter: "");
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
                          "User data",
                          style: TextStyle(
                            fontSize: 30,
                            fontWeight: FontWeight.w800,
                            color: UsersScreen.textColor,
                          ),
                        ),
                        SizedBox(height: 8),
                        Text(
                          "Browse all users, search by username or email, and navigate through pages.",
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
                    onPressed: _refreshUsers,
                  ),
                ],
              ),
              const SizedBox(height: 20),
              Row(
                children: [
                  Expanded(
                    child: _SearchBox(
                      controller: _searchController,
                      hintText: "Search username or email...",
                      onSubmitted: (_) => _searchUsers(),
                    ),
                  ),
                  const SizedBox(width: 12),
                  SizedBox(
                    height: 46,
                    child: ElevatedButton.icon(
                      onPressed: _paging.isLoading ? null : _searchUsers,
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
              _HeaderRow(),
              const SizedBox(height: 10),
              Expanded(child: _buildContent()),
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

  Widget _buildContent() {
    if (_paging.isLoading && _paging.items.isEmpty) {
      return const Center(child: CircularProgressIndicator());
    }

    if (_paging.items.isEmpty) {
      return const _EmptyState(
        title: "No users found",
        subtitle: "Try another search term or refresh the list.",
      );
    }

    return ListView.separated(
      itemCount: _paging.items.length,
      separatorBuilder: (_, __) => const SizedBox(height: 10),
      itemBuilder: (context, index) {
        final user = _paging.items[index];

        return _UserRow(
          username: _readUsername(user),
          email: _readEmail(user),
          type: _readType(user),
          status: _readStatus(user),
          isActive: _readIsActive(user),
        );
      },
    );
  }

  String _readUsername(dynamic user) {
    try {
      final value =
          user.username ??
          user.userName ??
          user.name ??
          user.fullName ??
          user.displayName;
      if (value != null && value.toString().trim().isNotEmpty) {
        return value.toString();
      }
    } catch (_) {}
    return "Unknown";
  }

  String _readEmail(dynamic user) {
    try {
      final value = user.email ?? user.mail;
      if (value != null && value.toString().trim().isNotEmpty) {
        return value.toString();
      }
    } catch (_) {}
    return "-";
  }

  String _readType(dynamic user) {
    try {
      final value =
          user.type ??
          user.userType ??
          user.subscriptionType ??
          user.membershipType ??
          user.roleName;
      if (value != null && value.toString().trim().isNotEmpty) {
        return value.toString();
      }
    } catch (_) {}

    try {
      final isPremium = user.isPremium;
      if (isPremium == true) return "Premium";
      if (isPremium == false) return "Basic";
    } catch (_) {}

    return "Basic";
  }

  String _readStatus(dynamic user) {
    try {
      final value = user.status;
      if (value != null && value.toString().trim().isNotEmpty) {
        return value.toString();
      }
    } catch (_) {}

    return _readIsActive(user) ? "Active" : "Inactive";
  }

  bool _readIsActive(dynamic user) {
    try {
      final value = user.isActive;
      if (value is bool) return value;
    } catch (_) {}

    try {
      final status = user.status?.toString().toLowerCase();
      if (status == "active") return true;
      if (status == "inactive") return false;
    } catch (_) {}

    return false;
  }
}

class _HeaderRow extends StatelessWidget {
  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 14),
      decoration: BoxDecoration(
        color: const Color(0xFFF9F9FB),
        border: Border.all(color: UsersScreen.borderColor),
        borderRadius: BorderRadius.circular(10),
      ),
      child: const Row(
        children: [
          Expanded(
            flex: 2,
            child: Text(
              "Username",
              style: TextStyle(
                fontWeight: FontWeight.w700,
                color: UsersScreen.textColor,
              ),
            ),
          ),
          Expanded(
            flex: 3,
            child: Text(
              "Email",
              style: TextStyle(
                fontWeight: FontWeight.w700,
                color: UsersScreen.textColor,
              ),
            ),
          ),
          Expanded(
            flex: 2,
            child: Text(
              "Type",
              style: TextStyle(
                fontWeight: FontWeight.w700,
                color: UsersScreen.textColor,
              ),
            ),
          ),
          Expanded(
            flex: 2,
            child: Text(
              "Status",
              style: TextStyle(
                fontWeight: FontWeight.w700,
                color: UsersScreen.textColor,
              ),
            ),
          ),
        ],
      ),
    );
  }
}

class _UserRow extends StatelessWidget {
  final String username;
  final String email;
  final String type;
  final String status;
  final bool isActive;

  const _UserRow({
    required this.username,
    required this.email,
    required this.type,
    required this.status,
    required this.isActive,
  });

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 14),
      decoration: BoxDecoration(
        color: Colors.white,
        border: Border.all(color: UsersScreen.borderColor),
        borderRadius: BorderRadius.circular(10),
      ),
      child: Row(
        children: [
          Expanded(
            flex: 2,
            child: Text(
              username,
              style: const TextStyle(
                fontWeight: FontWeight.w600,
                color: UsersScreen.textColor,
              ),
            ),
          ),
          Expanded(
            flex: 3,
            child: Text(
              email,
              style: const TextStyle(color: UsersScreen.subTextColor),
            ),
          ),
          Expanded(
            flex: 2,
            child: Text(
              type,
              style: const TextStyle(color: UsersScreen.textColor),
            ),
          ),
          Expanded(
            flex: 2,
            child: Align(
              alignment: Alignment.centerLeft,
              child: Container(
                padding: const EdgeInsets.symmetric(
                  horizontal: 10,
                  vertical: 6,
                ),
                decoration: BoxDecoration(
                  color: isActive
                      ? const Color(0xFFEFFAF1)
                      : const Color(0xFFFFF0F0),
                  borderRadius: BorderRadius.circular(999),
                ),
                child: Text(
                  status,
                  style: TextStyle(
                    color: isActive
                        ? const Color(0xFF2E7D32)
                        : const Color(0xFFC62828),
                    fontWeight: FontWeight.w700,
                    fontSize: 12,
                  ),
                ),
              ),
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
          style: const TextStyle(fontSize: 12, color: UsersScreen.subTextColor),
        ),
        const Spacer(),
        OutlinedButton.icon(
          onPressed: (!hasPreviousPage || isLoading)
              ? null
              : () => onPrevious(),
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

  const _RefreshIconButton({required this.onPressed, this.isLoading = false});

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

  const _EmptyState({required this.title, required this.subtitle});

  @override
  Widget build(BuildContext context) {
    return const Center(
      child: Padding(
        padding: EdgeInsets.all(24),
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            Icon(
              Icons.people_alt_outlined,
              size: 46,
              color: UsersScreen.subTextColor,
            ),
            SizedBox(height: 14),
          ],
        ),
      ),
    );
  }
}
