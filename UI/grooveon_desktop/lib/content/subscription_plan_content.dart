import 'package:flutter/material.dart';
import 'package:grooveon_desktop/dialogs/base_dialogs_frame.dart';
import 'package:grooveon_desktop/dialogs/confirmation_dialogs.dart';
import 'package:grooveon_desktop/helper/snackBar_helper.dart';
import 'package:grooveon_desktop/helper/universal_paging_helper.dart';
import 'package:grooveon_desktop/models/response/search_result.dart';
import 'package:grooveon_desktop/models/response/subscription_plan_response.dart';
import 'package:grooveon_desktop/providers/subscription_plan_provider.dart';
import 'package:grooveon_desktop/validation/user_validation.dart';

class SubscriptionPlanContent extends StatefulWidget {
  const SubscriptionPlanContent({super.key});

  @override
  State<SubscriptionPlanContent> createState() =>
      _SubscriptionPlanContentState();
}

class _SubscriptionPlanContentState extends State<SubscriptionPlanContent> {
  static const Color _primary = Color(0xFF9C27B0);
  static const Color _border = Color(0xFFD9D9DE);
  static const Color _text = Color(0xFF222222);
  static const Color _subText = Color(0xFF6F6F78);

  final _searchController = TextEditingController();
  late final SubscriptionPlanProvider _provider;
  late final UniversalPagingProvider<SubscriptionPlanResponse> _paging;

  @override
  void initState() {
    super.initState();
    _provider = SubscriptionPlanProvider();
    _paging = UniversalPagingProvider<SubscriptionPlanResponse>(
      pageSize: 8,
      fetcher: ({
        required int page,
        required int pageSize,
        String? filter,
        bool includeTotalCount = true,
      }) async {
        final map = {
          'page': page,
          'pageSize': pageSize,
          'includeTotalCount': includeTotalCount,
        };
        if (filter != null && filter.trim().isNotEmpty) {
          map['Name'] = filter.trim();
        }
        final result = await _provider.get(filter: map);
        return SearchResult<SubscriptionPlanResponse>(
          items: result.items.cast<SubscriptionPlanResponse>(),
          totalCount: result.totalCount,
        );
      },
    );
    WidgetsBinding.instance.addPostFrameCallback((_) => _paging.loadPage());
  }

  @override
  void dispose() {
    _searchController.dispose();
    _paging.dispose();
    super.dispose();
  }

  Future<void> _showForm({SubscriptionPlanResponse? item}) async {
    final nameCtrl = TextEditingController(text: item?.name ?? '');
    final codeCtrl = TextEditingController(text: item?.planCode ?? '');
    final priceCtrl =
        TextEditingController(text: item != null ? item.price.toString() : '');
    final daysCtrl = TextEditingController(
        text: item != null ? item.durationDays.toString() : '');
    final descCtrl = TextEditingController(text: item?.description ?? '');
    bool isActive = item?.isActive ?? true;
    final formKey = GlobalKey<FormState>();

    await showDialog(
      context: context,
      barrierDismissible: false,
      builder: (_) => StatefulBuilder(
        builder: (ctx, setDlg) => BaseDialog(
          title: item == null ? 'Add Plan' : 'Edit Plan',
          width: 480,
          onClose: () => Navigator.pop(ctx),
          child: Padding(
            padding: const EdgeInsets.all(20),
            child: Form(
              key: formKey,
              child: Column(
                mainAxisSize: MainAxisSize.min,
                children: [
                  _field('Name', nameCtrl, required: true),
                  const SizedBox(height: 12),
                  _field('Code', codeCtrl, required: true),
                  const SizedBox(height: 12),
                  Row(
                    children: [
                      Expanded(
                          child: _field('Price (€)', priceCtrl,
                              isPositiveDecimal: true)),
                      const SizedBox(width: 12),
                      Expanded(
                          child: _field('Duration (days)', daysCtrl,
                              isPositiveInteger: true)),
                    ],
                  ),
                  const SizedBox(height: 12),
                  _field('Description', descCtrl),
                  const SizedBox(height: 12),
                  Row(
                    children: [
                      Checkbox(
                        value: isActive,
                        activeColor: _primary,
                        onChanged: (v) => setDlg(() => isActive = v ?? true),
                      ),
                      const Text('Active',
                          style: TextStyle(fontSize: 14, color: _text)),
                    ],
                  ),
                  const SizedBox(height: 16),
                  Row(
                    mainAxisAlignment: MainAxisAlignment.end,
                    children: [
                      OutlinedButton(
                        onPressed: () => Navigator.pop(ctx),
                        style: OutlinedButton.styleFrom(
                          side: const BorderSide(color: _border),
                          padding: const EdgeInsets.symmetric(
                              horizontal: 18, vertical: 12),
                          shape: RoundedRectangleBorder(
                              borderRadius: BorderRadius.circular(10)),
                        ),
                        child: const Text('Cancel'),
                      ),
                      const SizedBox(width: 12),
                      ElevatedButton(
                        style: ElevatedButton.styleFrom(
                          backgroundColor: _primary,
                          foregroundColor: Colors.white,
                          elevation: 0,
                          padding: const EdgeInsets.symmetric(
                              horizontal: 18, vertical: 12),
                          shape: RoundedRectangleBorder(
                              borderRadius: BorderRadius.circular(10)),
                        ),
                        onPressed: () async {
                          if (!formKey.currentState!.validate()) return;
                          final payload = {
                            'name': nameCtrl.text.trim(),
                            'planCode': codeCtrl.text.trim().toLowerCase(),
                            'price': double.tryParse(priceCtrl.text) ?? 0,
                            'durationDays':
                                int.tryParse(daysCtrl.text) ?? 0,
                            'description': descCtrl.text.trim(),
                            'isActive': isActive,
                          };
                          try {
                            if (item == null) {
                              await _provider.insert(payload);
                            } else {
                              await _provider.update(item.id, payload);
                            }
                            if (!ctx.mounted) return;
                            Navigator.pop(ctx);
                            await _paging.loadPage();
                            if (!mounted) return;
                            SnackbarHelper.showSuccess(
                                context,
                                item == null
                                    ? 'Plan added.'
                                    : 'Plan updated.');
                          } catch (e) {
                            if (!ctx.mounted) return;
                            SnackbarHelper.showError(ctx, e.toString());
                          }
                        },
                        child: Text(
                          item == null ? 'Add' : 'Save',
                          style: const TextStyle(fontWeight: FontWeight.w700),
                        ),
                      ),
                    ],
                  ),
                ],
              ),
            ),
          ),
        ),
      ),
    );
  }

  Future<void> _delete(SubscriptionPlanResponse item) async {
    final ok = await ConfirmDialogs.yesNoConfirmation(
      context,
      title: 'Delete Plan',
      question: 'Delete plan "${item.name}"?',
      yesText: 'Delete',
      noText: 'Cancel',
    );
    if (!ok) return;
    try {
      await _provider.delete(item.id);
      await _paging.loadPage();
      if (!mounted) return;
      SnackbarHelper.showSuccess(context, 'Plan deleted.');
    } catch (e) {
      if (!mounted) return;
      SnackbarHelper.showError(context, e.toString());
    }
  }

  Widget _field(
    String label,
    TextEditingController ctrl, {
    bool required = false,
    bool isPositiveDecimal = false,
    bool isPositiveInteger = false,
  }) {
    final isNumber = isPositiveDecimal || isPositiveInteger;
    return TextFormField(
      controller: ctrl,
      keyboardType: isNumber
          ? const TextInputType.numberWithOptions(decimal: true)
          : TextInputType.text,
      validator: (v) {
        if (isPositiveDecimal) return Validators.positiveDecimal(v, label);
        if (isPositiveInteger) return Validators.positiveInteger(v, label);
        if (required && (v == null || v.trim().isEmpty)) {
          return '$label is required.';
        }
        return null;
      },
      decoration: InputDecoration(
        labelText: label,
        filled: true,
        fillColor: const Color(0xFFF9F9FB),
        border: OutlineInputBorder(borderRadius: BorderRadius.circular(12)),
        enabledBorder: OutlineInputBorder(
          borderRadius: BorderRadius.circular(12),
          borderSide: const BorderSide(color: _border),
        ),
      ),
    );
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
            color: Colors.white,
            border: Border.all(color: _border),
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
                        Text('Subscription Plans',
                            style: TextStyle(
                                fontSize: 28,
                                fontWeight: FontWeight.w800,
                                color: _text)),
                        SizedBox(height: 6),
                        Text(
                            'Manage subscription plans and pricing.',
                            style: TextStyle(
                                fontSize: 13, color: _subText)),
                      ],
                    ),
                  ),
                  ElevatedButton.icon(
                    onPressed: () => _showForm(),
                    icon: const Icon(Icons.add_rounded, size: 18),
                    label: const Text('Add Plan'),
                    style: ElevatedButton.styleFrom(
                      backgroundColor: _primary,
                      foregroundColor: Colors.white,
                      elevation: 0,
                      padding: const EdgeInsets.symmetric(
                          horizontal: 16, vertical: 12),
                      shape: RoundedRectangleBorder(
                          borderRadius: BorderRadius.circular(12)),
                    ),
                  ),
                ],
              ),
              const SizedBox(height: 20),
              Row(
                children: [
                  Expanded(
                    child: TextField(
                      controller: _searchController,
                      onSubmitted: (_) =>
                          _paging.search(_searchController.text.trim()),
                      decoration: InputDecoration(
                        hintText: 'Search plans...',
                        prefixIcon: const Icon(Icons.search_rounded),
                        filled: true,
                        fillColor: const Color(0xFFF8F8FA),
                        contentPadding: const EdgeInsets.symmetric(
                            horizontal: 14, vertical: 14),
                        border: OutlineInputBorder(
                            borderRadius: BorderRadius.circular(12),
                            borderSide: const BorderSide(color: _border)),
                        enabledBorder: OutlineInputBorder(
                            borderRadius: BorderRadius.circular(12),
                            borderSide: const BorderSide(color: _border)),
                        focusedBorder: OutlineInputBorder(
                          borderRadius: BorderRadius.circular(12),
                          borderSide:
                              const BorderSide(color: _primary, width: 1.2),
                        ),
                      ),
                    ),
                  ),
                  const SizedBox(width: 12),
                  SizedBox(
                    height: 46,
                    child: ElevatedButton.icon(
                      onPressed: _paging.isLoading
                          ? null
                          : () =>
                              _paging.search(_searchController.text.trim()),
                      icon: const Icon(Icons.search_rounded, size: 18),
                      label: const Text('Search'),
                      style: ElevatedButton.styleFrom(
                        backgroundColor: _primary,
                        foregroundColor: Colors.white,
                        elevation: 0,
                        padding:
                            const EdgeInsets.symmetric(horizontal: 18),
                        shape: RoundedRectangleBorder(
                            borderRadius: BorderRadius.circular(12)),
                      ),
                    ),
                  ),
                ],
              ),
              const SizedBox(height: 16),
              _PlanHeaderRow(),
              const SizedBox(height: 8),
              Expanded(child: _buildList()),
              const SizedBox(height: 12),
              _PagingBar(paging: _paging),
            ],
          ),
        );
      },
    );
  }

  Widget _buildList() {
    if (_paging.isLoading && _paging.items.isEmpty) {
      return const Center(child: CircularProgressIndicator());
    }
    if (_paging.items.isEmpty) {
      return const Center(
        child: Text('No plans found.',
            style: TextStyle(color: _subText, fontSize: 14)),
      );
    }
    return ListView.separated(
      itemCount: _paging.items.length,
      separatorBuilder: (_, _) => const SizedBox(height: 8),
      itemBuilder: (context, i) {
        final item = _paging.items[i];
        return _PlanRow(
          plan: item,
          onEdit: () => _showForm(item: item),
          onDelete: () => _delete(item),
        );
      },
    );
  }
}

class _PlanHeaderRow extends StatelessWidget {
  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 12),
      decoration: BoxDecoration(
        color: const Color(0xFFF9F9FB),
        border: Border.all(color: const Color(0xFFD9D9DE)),
        borderRadius: BorderRadius.circular(10),
      ),
      child: const Row(
        children: [
          Expanded(
              flex: 3,
              child: Text('Name',
                  style: TextStyle(
                      fontWeight: FontWeight.w700,
                      color: Color(0xFF222222)))),
          Expanded(
              flex: 2,
              child: Text('Price',
                  style: TextStyle(
                      fontWeight: FontWeight.w700,
                      color: Color(0xFF222222)))),
          Expanded(
              flex: 2,
              child: Text('Duration',
                  style: TextStyle(
                      fontWeight: FontWeight.w700,
                      color: Color(0xFF222222)))),
          Expanded(
              flex: 1,
              child: Text('Active',
                  style: TextStyle(
                      fontWeight: FontWeight.w700,
                      color: Color(0xFF222222)))),
          SizedBox(width: 80),
        ],
      ),
    );
  }
}

class _PlanRow extends StatelessWidget {
  final SubscriptionPlanResponse plan;
  final VoidCallback onEdit;
  final VoidCallback onDelete;

  const _PlanRow({
    required this.plan,
    required this.onEdit,
    required this.onDelete,
  });

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 12),
      decoration: BoxDecoration(
        color: Colors.white,
        border: Border.all(color: const Color(0xFFD9D9DE)),
        borderRadius: BorderRadius.circular(10),
      ),
      child: Row(
        children: [
          Expanded(
              flex: 3,
              child: Text(plan.name,
                  style: const TextStyle(
                      fontWeight: FontWeight.w600,
                      color: Color(0xFF222222)))),
          Expanded(
              flex: 2,
              child: Text('€${plan.price.toStringAsFixed(2)}',
                  style: const TextStyle(color: Color(0xFF6F6F78)))),
          Expanded(
              flex: 2,
              child: Text('${plan.durationDays} days',
                  style: const TextStyle(color: Color(0xFF6F6F78)))),
          Expanded(
            flex: 1,
            child: Container(
              padding:
                  const EdgeInsets.symmetric(horizontal: 8, vertical: 4),
              decoration: BoxDecoration(
                color: plan.isActive
                    ? const Color(0xFFEFFAF1)
                    : const Color(0xFFFFF0F0),
                borderRadius: BorderRadius.circular(99),
              ),
              child: Text(
                plan.isActive ? 'Active' : 'Inactive',
                textAlign: TextAlign.center,
                style: TextStyle(
                  fontSize: 11,
                  fontWeight: FontWeight.w700,
                  color: plan.isActive
                      ? const Color(0xFF2E7D32)
                      : const Color(0xFFC62828),
                ),
              ),
            ),
          ),
          SizedBox(
            width: 80,
            child: Row(
              mainAxisAlignment: MainAxisAlignment.end,
              children: [
                IconButton(
                  icon: const Icon(Icons.edit_outlined,
                      size: 18, color: Color(0xFF9C27B0)),
                  onPressed: onEdit,
                  tooltip: 'Edit',
                  padding: EdgeInsets.zero,
                  constraints: const BoxConstraints(),
                ),
                const SizedBox(width: 8),
                IconButton(
                  icon: const Icon(Icons.delete_outline,
                      size: 18, color: Colors.redAccent),
                  onPressed: onDelete,
                  tooltip: 'Delete',
                  padding: EdgeInsets.zero,
                  constraints: const BoxConstraints(),
                ),
              ],
            ),
          ),
        ],
      ),
    );
  }
}

class _PagingBar extends StatelessWidget {
  final UniversalPagingProvider paging;

  const _PagingBar({required this.paging});

  @override
  Widget build(BuildContext context) {
    final from =
        paging.totalCount == 0 ? 0 : (paging.page * paging.pageSize) + 1;
    final to = ((paging.page + 1) * paging.pageSize) > paging.totalCount
        ? paging.totalCount
        : (paging.page + 1) * paging.pageSize;

    return Row(
      children: [
        Text(
          paging.totalCount == 0
              ? 'No records'
              : 'Showing $from–$to of ${paging.totalCount}',
          style:
              const TextStyle(fontSize: 12, color: Color(0xFF6F6F78)),
        ),
        const Spacer(),
        OutlinedButton.icon(
          onPressed: paging.hasPreviousPage && !paging.isLoading
              ? () => paging.previousPage()
              : null,
          icon: const Icon(Icons.chevron_left_rounded),
          label: const Text('Previous'),
          style: OutlinedButton.styleFrom(
              side: const BorderSide(color: Color(0xFFD9D9DE))),
        ),
        const SizedBox(width: 8),
        OutlinedButton.icon(
          onPressed: paging.hasNextPage && !paging.isLoading
              ? () => paging.nextPage()
              : null,
          icon: const Icon(Icons.chevron_right_rounded),
          label: const Text('Next'),
          style: OutlinedButton.styleFrom(
              side: const BorderSide(color: Color(0xFFD9D9DE))),
        ),
      ],
    );
  }
}
